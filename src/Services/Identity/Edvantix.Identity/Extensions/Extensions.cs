using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Identity.Configurations;
using Edvantix.Identity.Infrastructure.Keycloak;
using Wolverine.EntityFrameworkCore;
using Wolverine.Persistence;
using Wolverine.Postgresql;

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

        services.AddEventBus(
            typeof(IIdentityApiMarker),
            options =>
            {
                var connectionString = builder.Configuration.GetRequiredConnectionString(
                    Components.Database.Identity
                );

                options.PersistMessagesWithPostgresql(connectionString);
                options.UseEntityFrameworkCoreTransactions(TransactionMiddlewareMode.Lightweight);

                options.Policies.AutoApplyTransactions();
            }
        );

        services.AddKeycloakTokenIntrospection();
    }
}
