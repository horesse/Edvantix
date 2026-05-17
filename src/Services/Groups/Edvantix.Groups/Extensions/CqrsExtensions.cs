using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Groups.Infrastructure.Services;

namespace Edvantix.Groups.Extensions;

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
                .ApplyAuthorizationBehavior()
                .ApplyTransactionBehavior<GroupsDbContext>();

            services.AddValidatorsFromAssemblyContaining<IGroupsApiMarker>(
                includeInternalTypes: true
            );

            services.AddActivityScope().AddCommandHandlerMetrics().AddQueryHandlerMetrics();

            services.AddEventDispatcher();
            services.AddScoped<IEventMapper, EventMapper>();
        }
    }
}
