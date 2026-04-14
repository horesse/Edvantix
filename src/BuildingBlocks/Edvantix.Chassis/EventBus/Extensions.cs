using System.Reflection;
using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Chassis.EventBus.Kafka;
using Edvantix.Chassis.EventBus.Serialization;
using Edvantix.Constants.Aspire;
using FluentValidation;
using MassTransit;
using MassTransit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edvantix.Chassis.EventBus;

public static class Extensions
{
    private static readonly KebabCaseEndpointNameFormatter _formatter = new(false);

    private static void AddRetryConfiguration(IRetryConfigurator retryConfigurator)
    {
        retryConfigurator
            .Exponential(
                3,
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMinutes(120),
                TimeSpan.FromMilliseconds(200)
            )
            .Ignore<ValidationException>();
    }

    private static List<ConsumerEntry> DiscoverConsumerEntries(Assembly assembly)
    {
        var consumerInterface = typeof(IConsumer<>);

        return
        [
            .. assembly
                .GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false, IsClass: true })
                .SelectMany(t =>
                    t.GetInterfaces()
                        .Where(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == consumerInterface
                        )
                        .Select(i => new ConsumerEntry(t, i.GetGenericArguments()[0]))
                ),
        ];
    }

    private static void RegisterKafkaConsumers(
        IRiderRegistrationConfigurator rider,
        List<ConsumerEntry> consumerEntries
    )
    {
        foreach (
            var registrar in consumerEntries
                .Select(entry => typeof(ConsumerRegistrar<>).MakeGenericType(entry.ConsumerType))
                .Select(registrarType =>
                    (IConsumerRegistrar)Activator.CreateInstance(registrarType)!
                )
        )
        {
            registrar.Register(rider);
        }
    }

    private static void RegisterKafkaProducers(
        IRiderRegistrationConfigurator rider,
        Assembly assembly
    )
    {
        var messageTypes = DiscoverMessageTypes(assembly);

        foreach (var messageType in messageTypes)
        {
            var topicName = _formatter.SanitizeName(messageType.Name);
            var registrarType = typeof(ProducerRegistrar<>).MakeGenericType(messageType);
            var registrar = (IProducerRegistrar)Activator.CreateInstance(registrarType)!;
            registrar.Register(rider, topicName);
        }
    }

    private static void ConfigureKafkaTopicEndpoints(
        IKafkaFactoryConfigurator kafka,
        IRiderRegistrationContext context,
        Type assemblyMarker,
        List<ConsumerEntry> consumerEntries
    )
    {
        var consumerGroup = _formatter.SanitizeName(assemblyMarker.Assembly.GetName().Name!);

        foreach (var entry in consumerEntries)
        {
            var topicName = _formatter.SanitizeName(entry.MessageType.Name);
            var configuratorType = typeof(TopicEndpointConfigurator<,>).MakeGenericType(
                entry.MessageType,
                entry.ConsumerType
            );
            var configurator = (ITopicEndpointConfigurator)
                Activator.CreateInstance(configuratorType)!;
            configurator.Configure(kafka, context, topicName, consumerGroup);
        }
    }

    private static HashSet<Type> DiscoverMessageTypes(Assembly assembly)
    {
        var integrationEventType = typeof(IntegrationEvent);
        var consumerInterface = typeof(IConsumer<>);
        var messageTypes = new HashSet<Type>();

        foreach (var type in assembly.GetTypes())
        {
            if (
                type is { IsAbstract: false, IsInterface: false }
                && integrationEventType.IsAssignableFrom(type)
            )
            {
                messageTypes.Add(type);
            }

            if (type.IsAbstract || type.IsInterface || !type.IsClass)
            {
                continue;
            }

            foreach (var face in type.GetInterfaces())
            {
                if (face.IsGenericType && face.GetGenericTypeDefinition() == consumerInterface)
                {
                    messageTypes.Add(face.GetGenericArguments()[0]);
                }
            }
        }

        return messageTypes;
    }

    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует и настраивает инфраструктуру шины событий для текущего хоста.
        /// </summary>
        /// <param name="type">Маркерный тип для обнаружения потребителей, производителей и активностей из его сборки.</param>
        /// <param name="busConfigure">Необязательный обратный вызов для дополнительной настройки <see cref="IBusRegistrationConfigurator" />.</param>
        /// <remarks>
        /// Метод настраивает in-memory транспорт MassTransit для локального конвейера
        /// и Kafka rider для брокерного обмена сообщениями. Если строка подключения к брокеру не настроена, регистрация пропускается.
        /// </remarks>
        public void AddEventBus(
            Type type,
            Action<IBusRegistrationConfigurator>? busConfigure = null
        )
        {
            // Считывает параметры подключения к брокеру из конфигурации.
            var connectionString = builder.Configuration.GetConnectionString(Components.Broker);

            // Пропускает регистрацию шины событий, если брокер не настроен.
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return;
            }

            builder.Services.AddMassTransit(config =>
            {
                // Устанавливает форматирование имён эндпоинтов в kebab-case для единообразия.
                config.SetKebabCaseEndpointNameFormatter();

                // Автоматически регистрирует активности MassTransit из целевой сборки.
                config.AddActivities(type.Assembly);

                config.UsingInMemory(
                    (context, configurator) =>
                    {
                        // Использует конверты CloudEvents для совместимости.
                        configurator.UseCloudEvents();

                        // Автоматически настраивает эндпоинты обнаруженных потребителей.
                        configurator.ConfigureEndpoints(context);

                        // Применяет общую политику повторных попыток и игнорирует исключения валидации.
                        configurator.UseMessageRetry(AddRetryConfiguration);

                        // Включает отложенное планирование для доставки сообщений с задержкой.
                        configurator.UseDelayedMessageScheduler();

                        // Внедряет идентификатор пользователя из текущего HTTP-контекста в заголовки сообщения.
                        configurator.UsePublishFilter(typeof(UserIdPublishFilter<>), context);

                        // Перенаправляет операции публикации через Kafka с помощью фильтра публикации.
                        configurator.UsePublishFilter(typeof(KafkaPublishFilter<>), context);
                    }
                );

                config.AddRider(rider =>
                {
                    // Обнаруживает потребителей один раз и переиспользует результат для регистрации и настройки эндпоинтов.
                    var consumerEntries = DiscoverConsumerEntries(type.Assembly);

                    // Регистрирует всех обнаруженных Kafka-потребителей и производителей.
                    RegisterKafkaConsumers(rider, consumerEntries);
                    RegisterKafkaProducers(rider, type.Assembly);

                    rider.UsingKafka(
                        (context, k) =>
                        {
                            // Настраивает хост Kafka и стратегию сериализации.
                            k.Host(connectionString);
                            k.SetSerializationFactory(new CloudEventKafkaSerializerFactory());

                            // Создаёт эндпоинты топиков и привязывает каждого потребителя к его топику сообщений.
                            ConfigureKafkaTopicEndpoints(k, context, type, consumerEntries);
                        }
                    );
                });

                // Позволяет вызывающему коду добавлять дополнительную конфигурацию MassTransit.
                busConfigure?.Invoke(config);
            });

            // Регистрирует счётчик/источник диагностики MassTransit для метрик и трассировок OpenTelemetry.
            builder
                .Services.AddOpenTelemetry()
                .WithMetrics(b => b.AddMeter(DiagnosticHeaders.DefaultListenerName))
                .WithTracing(p => p.AddSource(DiagnosticHeaders.DefaultListenerName));
        }
    }

    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует сервис диспетчера событий в контейнере зависимостей.
        /// </summary>
        /// <remarks>
        /// Диспетчер регистрируется с областью видимости scoped, поэтому один экземпляр используется на всё время запроса.
        /// </remarks>
        public void AddEventDispatcher()
        {
            services.AddScoped<IEventDispatcher, EventDispatcher>();
        }
    }

    private sealed record ConsumerEntry(Type ConsumerType, Type MessageType);

    private interface IConsumerRegistrar
    {
        void Register(IRiderRegistrationConfigurator rider);
    }

    private sealed class ConsumerRegistrar<TConsumer> : IConsumerRegistrar
        where TConsumer : class, IConsumer
    {
        public void Register(IRiderRegistrationConfigurator rider)
        {
            rider.AddConsumer<TConsumer>();
        }
    }

    private interface IProducerRegistrar
    {
        void Register(IRiderRegistrationConfigurator rider, string topicName);
    }

    private sealed class ProducerRegistrar<TMessage> : IProducerRegistrar
        where TMessage : class
    {
        public void Register(IRiderRegistrationConfigurator rider, string topicName)
        {
            rider.AddProducer<TMessage>(topicName);
        }
    }

    private interface ITopicEndpointConfigurator
    {
        void Configure(
            IKafkaFactoryConfigurator kafka,
            IRiderRegistrationContext context,
            string topicName,
            string consumerGroup
        );
    }

    private sealed class TopicEndpointConfigurator<TMessage, TConsumer> : ITopicEndpointConfigurator
        where TMessage : class
        where TConsumer : class, IConsumer<TMessage>
    {
        public void Configure(
            IKafkaFactoryConfigurator kafka,
            IRiderRegistrationContext context,
            string topicName,
            string consumerGroup
        )
        {
            kafka.TopicEndpoint<TMessage>(
                topicName,
                consumerGroup,
                e =>
                {
                    e.CreateIfMissing();
                    e.ConfigureConsumer<TConsumer>(context);
                }
            );
        }
    }
}
