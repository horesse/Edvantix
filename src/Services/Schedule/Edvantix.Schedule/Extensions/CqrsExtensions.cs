using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Schedule.Infrastructure.Services;

namespace Edvantix.Schedule.Extensions;

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
                .ApplyTransactionBehavior<ScheduleDbContext>();

            services.AddValidatorsFromAssemblyContaining<IScheduleApiMarker>(
                includeInternalTypes: true
            );

            services.AddActivityScope().AddCommandHandlerMetrics().AddQueryHandlerMetrics();

            services.AddEventDispatcher();
            services.AddScoped<IEventMapper, EventMapper>();
        }
    }
}
