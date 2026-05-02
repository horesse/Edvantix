using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Constants.Aspire;
using FluentValidation;
using JasperFx;
using JasperFx.Resources;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Kafka;

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
                opts.Services.AddResourceSetupOnStartup();

                opts.AutoBuildMessageStorageOnStartup = AutoCreate.All;

                opts.UseKafkaUsingNamedConnection(Components.Broker).AutoProvision();

                opts.Policies.OnException<ValidationException>().Discard();

                opts.Policies.OnException<Exception>()
                    .RetryWithCooldown(
                        TimeSpan.FromMilliseconds(200),
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5)
                    );

                configure?.Invoke(opts);
            });

            services
                .AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing.AddSource(nameof(Wolverine));
                })
                .WithMetrics(metrics =>
                {
                    metrics.AddMeter(nameof(Wolverine));
                });
        }

        public void AddEventDispatcher()
        {
            services.AddScoped<IEventDispatcher, EventDispatcher>();
        }
    }
}
