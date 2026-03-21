using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Constants.Permissions;
using Edvantix.Scheduling.Configurations;
using Edvantix.Scheduling.Grpc;
using Edvantix.Scheduling.Infrastructure;
using Edvantix.Scheduling.Infrastructure.Seeding;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Scheduling.Extensions;

/// <summary>
/// Application service registration for the Scheduling service.
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
                    .RequireScope($"{Services.Scheduling}_{Authorization.Actions.Read}")
                    .Build()
            )
            // Authorization policies for each scheduling permission.
            // Each policy delegates to the Organizations gRPC permission check.
            .AddPolicy(
                SchedulingPermissions.CreateLessonSlot,
                p => p.RequirePermission(SchedulingPermissions.CreateLessonSlot)
            )
            .AddPolicy(
                SchedulingPermissions.EditLessonSlot,
                p => p.RequirePermission(SchedulingPermissions.EditLessonSlot)
            )
            .AddPolicy(
                SchedulingPermissions.DeleteLessonSlot,
                p => p.RequirePermission(SchedulingPermissions.DeleteLessonSlot)
            )
            .AddPolicy(
                SchedulingPermissions.ViewSchedule,
                p => p.RequirePermission(SchedulingPermissions.ViewSchedule)
            )
            .AddPolicy(
                SchedulingPermissions.ViewOwnSchedule,
                p => p.RequirePermission(SchedulingPermissions.ViewOwnSchedule)
            );

        // Wire Organizations gRPC client for permission checking at authorization layer.
        builder.AddPermissionAuthorization($"https+http://{Services.Organizations}");

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

        builder.AddAppSettings<SchedulingAppSettings>();

        builder.AddRateLimiting();

        builder.AddPersistenceServices();

        services.AddValidatorsFromAssemblyContaining<ISchedulingApiMarker>(
            includeInternalTypes: true
        );

        services.AddTransient(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);

        services.AddActivityScope().AddCommandHandlerMetrics().AddQueryHandlerMetrics();

        services.AddVersioning();
        services.AddEndpoints(typeof(ISchedulingApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<
                OpenApiInfoDefinitionsTransformer<SchedulingAppSettings>
            >()
        );

        // Wire Kafka outbox via MassTransit for future Phase 4 events.
        // No domain events in Phase 3 — outbox infrastructure is provisioned here for forward-compatibility.
        builder.AddEventBus(
            typeof(ISchedulingApiMarker),
            cfg =>
            {
                cfg.AddEntityFrameworkOutbox<SchedulingDbContext>(o =>
                {
                    o.QueryDelay = TimeSpan.FromSeconds(1);
                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
                    o.UsePostgres();
                    o.UseBusOutbox();
                });

                cfg.AddConfigureEndpointsCallback(
                    (context, _, configurator) =>
                        configurator.UseEntityFrameworkOutbox<SchedulingDbContext>(context)
                );
            }
        );

        services.AddKeycloakTokenIntrospection();

        // Wire Persona gRPC client for profileId validation in future teacher/student assignments.
        builder.AddGrpcServices();

        // Named HttpClient pointing at Organizations for permission seeding on startup.
        // StandardResilienceHandler applies retry + circuit breaker for transient failures
        // when Organizations has not yet fully started.
        services
            .AddHttpClient("organizations", c =>
                c.BaseAddress = new Uri($"http://{Services.Organizations}")
            )
            .AddStandardResilienceHandler();

        // Seed Scheduling permissions into Organizations' catalogue on startup via HTTP.
        services.AddHostedService<PermissionSeeder>();
    }
}
