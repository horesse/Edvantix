using Edvantix.Audit.Infrastructure.Services;
using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Chassis.OpenTelemetry;

namespace Edvantix.Audit.Extensions;

internal static class CqrsExtensions
{
    extension(IServiceCollection services)
    {
        public void AddCqrsInfrastructure()
        {
            services
                .AddMediator(
                    (MediatorOptions options) => options.ServiceLifetime = ServiceLifetime.Scoped
                )
                .ApplyLoggingBehavior()
                .ApplyActivityBehavior()
                .ApplyValidationBehavior()
                .ApplyTransactionBehavior<AuditDbContext>();

            services.AddValidatorsFromAssemblyContaining<IAuditApiMarker>(
                includeInternalTypes: true
            );

            services.AddActivityScope().AddCommandHandlerMetrics().AddQueryHandlerMetrics();

            services.AddEventDispatcher();
            services.AddScoped<IEventMapper, EventMapper>();
        }
    }
}
