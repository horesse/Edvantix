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

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        builder.AddAppSettings<IdentityAppSettings>();

        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = builder.Environment.IsDevelopment();
        });

        services.AddGrpcHealthChecks();

        builder.AddPersistenceServices();

        services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();

        builder.AddEventBus(
            typeof(IIdentityApiMarker),
            cfg =>
            {
                cfg.AddEntityFrameworkOutbox<IdentityDbContext>(o =>
                {
                    o.QueryDelay = TimeSpan.FromSeconds(1);

                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);

                    o.UsePostgres();

                    o.UseBusOutbox();
                });

                cfg.AddConfigureEndpointsCallback(
                    (context, _, configurator) =>
                        configurator.UseEntityFrameworkOutbox<IdentityDbContext>(context)
                );
            }
        );

        services.AddKeycloakTokenIntrospection();
    }
}
