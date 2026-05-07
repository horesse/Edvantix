using System.Reflection;
using System.Text.Json;
using Edvantix.Constants.Aspire;
using JasperFx.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Configuration;
using Wolverine.Kafka;

namespace Edvantix.Chassis.EventBus.Wolverine;

public static class WolverineHostExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует WolverineFx с долговечным Postgres outbox/inbox для каждого сервиса,
        /// Kafka-транспортом совместимым с CloudEvents и политиками распространения заголовков.
        /// Если строка подключения к брокеру не настроена, вызов является холостым,
        /// что позволяет сервисам запускаться без активного кластера Kafka.
        /// </summary>
        /// <param name="configure">
        /// Необязательный обратный вызов для дополнительной настройки <see cref="WolverineOptions" />
        /// (например, обнаружение обработчиков, специфичных для сервиса, или переопределение конечных точек).
        /// </param>
        public void UseEventFramework(Action<WolverineOptions>? configure = null)
        {
            var kafkaConnectionString = builder.Configuration.GetConnectionString(
                Components.Broker
            );

            if (string.IsNullOrWhiteSpace(kafkaConnectionString))
            {
                return;
            }

            builder.Services.AddSingleton<UserIdEnvelopeMiddleware>();

            var applicationName = builder.Environment.ApplicationName;

            builder.Services.AddWolverine(
                // Пропускаем сканирование всех загруженных DLL (включая нативные, такие как librdkafka.dll)
                // в поиске реализаций IWolverineExtension. Расширения подключаются явно через Include<T>() /
                // AddWolverineExtension<T>() при необходимости.
                ExtensionDiscovery.ManualOnly,
                opts =>
                {
                    // ── Долговечный outbox/inbox ──────────────────────────────────────────
                    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
                    opts.Policies.UseDurableInboxOnAllListeners();

                    // Обеспечивает создание схемы Wolverine (wolverine_incoming, wolverine_outgoing и т.д.)
                    // при первом запуске без ручного шага миграции.
                    opts.Services.AddResourceSetupOnStartup();

                    // Логируем начало каждого выполнения сообщения на уровне Debug, чтобы OTel/структурированные
                    // логи фиксировали поток сообщений без переполнения продуктовых логов.
                    opts.Policies.LogMessageStarting(LogLevel.Debug);

                    // ── Правила конверта (распространение заголовков) ───────────────────
                    // CloudEventHeaderPolicy проставляет messagetype, destinationaddress,
                    // responseaddress и URN источника CloudEvent (urn:edvantix:{service})
                    // в заголовки конверта перед запуском маппера CloudEvents.
                    var sourceUrn = $"urn:edvantix:{KafkaTopicRouter.ToKebabCase(applicationName)}";
                    opts.MetadataRules.Add(new CloudEventHeaderPolicy(sourceUrn));

                    // UserIdEnvelopeMiddleware проставляет HTTP-идентификатор пользователя в заголовки конверта.
                    // Разрешается через DI, чтобы использовался общий экземпляр IHttpContextAccessor.
                    opts.Services.AddSingleton<IEnvelopeRule, UserIdEnvelopeMiddleware>(sp =>
                        sp.GetRequiredService<UserIdEnvelopeMiddleware>()
                    );

                    // ── Kafka-транспорт с совместимостью CloudEvents ──────────────────────
                    // Регистрируется до обратного вызова конфигурации сервиса, чтобы вспомогательные
                    // методы (ListenToIntegrationEventsIn / ListenToKafkaTopic) можно было вызывать из него.
                    opts.UseKafkaWithCloudEvents(kafkaConnectionString, applicationName);

                    // ── Настройка для конкретного сервиса ─────────────────────────────────
                    configure?.Invoke(opts);
                }
            );

            builder
                .Services.AddOpenTelemetry()
                .WithTracing(tracing => tracing.AddSource("Wolverine").AddSource("Confluent.Kafka"))
                .WithMetrics(metrics => metrics.AddMeter("Wolverine"));
        }
    }

    extension(WolverineOptions opts)
    {
        private void UseKafkaWithCloudEvents(string kafkaConnectionString, string applicationName)
        {
            opts.ServiceName = KafkaTopicRouter.ToKebabCase(applicationName);

            var cloudEventsJsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            opts.UseKafka(kafkaConnectionString).AutoProvision();
            opts.Policies.Add(
                new LambdaEndpointPolicy<KafkaTopic>(
                    (topic, runtime) =>
                    {
                        var cloudEvents = topic.BuildCloudEventsMapper(
                            runtime,
                            cloudEventsJsonOptions
                        );
                        topic.EnvelopeMapper = new CloudEventsOnlyKafkaMapper(cloudEvents);
                        topic.DefaultSerializer = cloudEvents;
                    }
                )
            );

            opts.PublishAllMessages().ToKafkaTopics().TelemetryEnabled(true);
        }

        /// <summary>
        /// Сканирует указанные сборки на наличие публичных методов с единственным параметром
        /// типа <see cref="IntegrationEvent"/> и регистрирует слушатель Kafka для каждого
        /// найденного уникального типа сообщения. Слушатель подписывается на топик,
        /// указанный в <see cref="MessageIdentityAttribute"/> типа сообщения, или на полное
        /// имя типа, если атрибут отсутствует.
        /// </summary>
        /// <param name="assemblies">Список сборок для сканирования методов-обработчиков сообщений.</param>
        public void ListenToIntegrationEventsIn(params Assembly[] assemblies)
        {
            var topics = new Dictionary<string, Type>(StringComparer.Ordinal);

            foreach (var assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = [.. ex.Types.Where(t => t is not null).Cast<Type>()];
                }

                foreach (var type in types)
                {
                    if (type.IsAbstract || type.IsInterface)
                    {
                        continue;
                    }

                    foreach (
                        var method in type.GetMethods(
                            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static
                        )
                    )
                    {
                        if (
                            method.Name
                            is not ("Handle" or "HandleAsync" or "Consume" or "ConsumeAsync")
                        )
                        {
                            continue;
                        }

                        var parameters = method.GetParameters();
                        if (parameters.Length == 0)
                        {
                            continue;
                        }

                        var messageType = parameters[0].ParameterType;
                        if (!typeof(IntegrationEvent).IsAssignableFrom(messageType))
                        {
                            continue;
                        }

                        var identity = messageType.GetCustomAttribute<MessageIdentityAttribute>();
                        var topic = identity?.Alias ?? messageType.FullName ?? messageType.Name;
                        topics[topic] = messageType;
                    }
                }
            }

            foreach (var (topic, messageType) in topics)
            {
                // Предварительно регистрируем псевдоним типа сообщения, чтобы CloudEventsMapper мог
                // разрешить поле `type` входящего CloudEvent (например, "Edvantix.Contracts.FeedbackCreatedIntegrationEvent")
                // обратно в .NET-тип. Без этого регистрация через обнаружение обработчиков может пропустить
                // тип, и потребитель бросает UnknownMessageTypeNameException при десериализации.
                opts.RegisterMessageType(messageType, topic);

                opts.ListenToKafkaTopic(topic);
            }
        }
    }
}
