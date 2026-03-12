using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Persona.Features.Profiles.UpdateAvatar;
using Edvantix.Persona.Features.Profiles.UpdateProfileByAdmin;
using Edvantix.Persona.Infrastructure.EventServices;

namespace Edvantix.Persona.Extensions;

internal static class CqrsExtensions
{
    public static void AddCqrsInfrastructure(this IServiceCollection services)
    {
        // Configure Mediator
        services
            .AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped)
            .ApplyLoggingBehavior()
            .ApplyActivityBehavior()
            .ApplyValidationBehavior()
            .AddScoped<UpdateAvatarPreProcessor>()
            .AddScoped<UpdateAvatarPostProcessor>()
            .AddScoped<UpdateProfileByAdminPreProcessor>()
            .AddScoped<UpdateProfileByAdminPostProcessor>();

        // Configure FluentValidation
        services.AddValidatorsFromAssemblyContaining<IPersonaApiMarker>(
            includeInternalTypes: true
        );

        services.AddActivityScope().AddCommandHandlerMetrics().AddQueryHandlerMetrics();

        services.AddEventDispatcher();
        services.AddScoped<IEventMapper, EventMapper>();
        
        // services.AddScoped<IRequestManager, RequestManager>();
    }
}
