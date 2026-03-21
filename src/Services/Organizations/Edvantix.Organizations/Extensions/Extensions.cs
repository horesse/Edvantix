using Edvantix.Chassis.Caching;
using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Constants.Permissions;
using Edvantix.Organizations.Configurations;
using Edvantix.Organizations.Grpc;
using Edvantix.Organizations.Infrastructure;
using Edvantix.Organizations.Infrastructure.EventServices;
using Edvantix.Organizations.Infrastructure.Seeding;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Hybrid;

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
            )
            // Authorization policies for each Groups permission.
            // Organizations checks permissions against itself via the gRPC endpoint (self-call)
            // so that group management operations are guarded by the same RBAC system.
            .AddPolicy(
                GroupsPermissions.CreateGroup,
                p => p.RequirePermission(GroupsPermissions.CreateGroup)
            )
            .AddPolicy(
                GroupsPermissions.UpdateGroup,
                p => p.RequirePermission(GroupsPermissions.UpdateGroup)
            )
            .AddPolicy(
                GroupsPermissions.DeleteGroup,
                p => p.RequirePermission(GroupsPermissions.DeleteGroup)
            )
            .AddPolicy(
                GroupsPermissions.ManageGroupMembership,
                p => p.RequirePermission(GroupsPermissions.ManageGroupMembership)
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

        // HybridCache — L1 in-memory (30s) + L2 Redis (5min) for permission checks.
        // The configure callback overrides DefaultEntryOptions after the Caching section is applied.
        builder.AddCaching(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5), // L2 Redis TTL
                LocalCacheExpiration = TimeSpan.FromSeconds(30), // L1 in-memory TTL
            };
        });

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

        // Organizations checks group permissions against itself via gRPC (self-call pattern).
        // The PermissionRequirementHandler is wired here so that GroupsPermissions policies
        // resolve correctly at runtime using the RBAC system.
        builder.AddPermissionAuthorization($"https+http://{Services.Organizations}");

        // Seed Organizations' own permissions directly into the DB on startup.
        // Using IHostedService (not the HTTP endpoint) avoids the self-calling race condition.
        services.AddHostedService<PermissionSeeder>();
    }
}
