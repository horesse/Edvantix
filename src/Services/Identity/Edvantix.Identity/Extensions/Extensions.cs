using Edvantix.Chassis.EventBus.Wolverine;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Identity.Configurations;
using Edvantix.Identity.Infrastructure.Keycloak;

namespace Edvantix.Identity.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultAuthentication().WithKeycloakClaimsTransformation();

        services.AddGlobalExceptionHandler();
        services.AddProblemDetails();

        builder.AddAppSettings<IdentityAppSettings>();

        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = builder.Environment.IsDevelopment();
        });

        services.AddGrpcHealthChecks();

        builder.AddPersistenceServices();

        services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();

        builder.AddEventBus(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(IIdentityApiMarker).Assembly);
            opts.ListenToIntegrationEventsIn(typeof(IIdentityApiMarker).Assembly);
        });

        services.AddKeycloakTokenIntrospection();
    }
}
