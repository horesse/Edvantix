using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Organizations.Configurations;
using Edvantix.Organizations.Grpc;
using Edvantix.Organizations.Infrastructure;
using Edvantix.Organizations.Infrastructure.EventServices;
using Edvantix.Organizations.Infrastructure.Seeding;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Organizations.Extensions;

/// <summary>
/// Application service registration for the Organizations service.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Registers all application services including persistence, authentication,
    /// authorization, event bus, and API infrastructure.
    /// </summary>
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        builder.AddDefaultAuthentication().WithKeycloakClaimsTransformation();

        services
            .AddAuthorizationBuilder()
            .SetDefaultPolicy(
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireScope($"{Services.Organizations}_{Authorization.Actions.Read}")
                    .Build()
            );

        // Tenant context — scoped per request, populated by TenantMiddleware
        services.AddTenantContext();

        // Exception handlers in priority order: validation > not-found > global
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services
            .AddMediator(
                (MediatorOptions options) => options.ServiceLifetime = ServiceLifetime.Scoped
            )
            .ApplyActivityBehavior()
            .ApplyLoggingBehavior()
            .ApplyValidationBehavior();

        builder.AddAppSettings<OrganizationsAppSettings>();

        builder.AddRateLimiting();

        builder.AddPersistenceServices();

        services.AddValidatorsFromAssemblyContaining<IOrganizationsApiMarker>(
            includeInternalTypes: true
        );

        services.AddTransient(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);

        services.AddActivityScope().AddCommandHandlerMetrics().AddQueryHandlerMetrics();

        services.AddVersioning();
        services.AddEndpoints(typeof(IOrganizationsApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<
                OpenApiInfoDefinitionsTransformer<OrganizationsAppSettings>
            >()
        );

        services.AddMapper(typeof(IOrganizationsApiMarker));

        services.AddScoped<IEventMapper, EventMapper>();
        services.AddEventDispatcher();

        builder.AddEventBus(
            typeof(IOrganizationsApiMarker),
            cfg =>
            {
                cfg.AddEntityFrameworkOutbox<OrganizationsDbContext>(o =>
                {
                    o.QueryDelay = TimeSpan.FromSeconds(1);
                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
                    o.UsePostgres();
                    o.UseBusOutbox();
                });

                cfg.AddConfigureEndpointsCallback(
                    (context, _, configurator) =>
                        configurator.UseEntityFrameworkOutbox<OrganizationsDbContext>(context)
                );
            }
        );

        services.AddKeycloakTokenIntrospection();

        // Wire Persona gRPC client for profileId validation in role assignments
        builder.AddGrpcServices();

        // Seed Organizations' own permissions directly into the DB on startup.
        // Using IHostedService (not the HTTP endpoint) avoids the self-calling race condition.
        services.AddHostedService<PermissionSeeder>();
    }
}
