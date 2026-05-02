using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Constants.Aspire;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Kafka;
using Wolverine.Util;

namespace Edvantix.Chassis.EventBus;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public void AddEventBus(Type type, Action<WolverineOptions>? configure = null)
        {
            services.AddWolverine(opts =>
            {
                opts.Discovery.IncludeAssembly(type.Assembly);

                opts.UseKafkaUsingNamedConnection(Components.Broker).AutoProvision();

                opts.Policies.OnException<ValidationException>().Discard();

                opts.Policies.OnException<Exception>()
                    .RetryWithCooldown(
                        TimeSpan.FromMilliseconds(200),
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5)
                    );

                opts.RegisterProducers(type);

                configure?.Invoke(opts);
            });
        }

        public void AddEventDispatcher()
        {
            services.AddScoped<IEventDispatcher, EventDispatcher>();
        }
    }

    extension(WolverineOptions opts)
    {
        private void RegisterProducers(Type type)
        {
            var messageTypes = type
                .Assembly.GetTypes()
                .Where(t => typeof(IntegrationEvent).IsAssignableFrom(t));

            foreach (var messageType in messageTypes)
            {
                opts.PublishMessage(messageType)
                    .ToKafkaTopic(messageType.ToMessageTypeName())
                    .InteropWithCloudEvents();
            }
        }

        // TODO: делать регистрацию handler`ов.
        private void RegisterConsumers(Type type)
        {
            var messageTypes = type
                .Assembly.GetTypes()
                .Where(t => typeof(IntegrationEvent).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var messageType in messageTypes)
            {
                opts.ListenToKafkaTopic(messageType.ToMessageTypeName())
                    .InteropWithCloudEvents()
                    .ConfigureConsumer(c =>
                    {
                        c.GroupId = nameof(Edvantix);
                    });
            }
        }
    }
}
