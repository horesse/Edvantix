using System.Text.Json;
using Edvantix.Chassis.CQRS.Command;
using Edvantix.Chassis.CQRS.Pipelines;
using Edvantix.Chassis.CQRS.Query;
using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Persona.Features.Profiles.UpdateOwnProfile;
using Edvantix.Persona.Features.Profiles.UpdateProfileByAdmin;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Persona.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        builder.AddDefaultAuthentication().WithKeycloakClaimsTransformation();

        services
            .AddAuthorizationBuilder()
            .AddPolicy(
                Authorization.Policies.Admin,
                policy =>
                {
                    policy
                        .RequireAuthenticatedUser()
                        .RequireRole(Authorization.Roles.Admin)
                        .RequireScope(
                            $"{Services.Persona}_{Authorization.Actions.Read}",
                            $"{Services.Persona}_{Authorization.Actions.Write}"
                        );
                }
            )
            .SetDefaultPolicy(
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireScope($"{Services.Persona}_{Authorization.Actions.Read}")
                    .Build()
            );

        builder.AddDefaultOpenApi();

        // Add exception handlers
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddSingleton(
            new JsonSerializerOptions { Converters = { DateOnlyJsonConverter.Instance } }
        );

        services
            .AddMediator(
                (MediatorOptions options) => options.ServiceLifetime = ServiceLifetime.Scoped
            )
            // Open-generic behaviors run first (activity tracing → logging → validation)
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ActivityBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            // Avatar upload/delete processors — run after validation, before/after handler
            .AddScoped<
                IPipelineBehavior<UpdateOwnProfileCommand, ProfileViewModel>,
                UpdateOwnProfilePreProcessor
            >()
            .AddScoped<
                IPipelineBehavior<UpdateOwnProfileCommand, ProfileViewModel>,
                UpdateOwnProfilePostProcessor
            >()
            .AddScoped<
                IPipelineBehavior<UpdateProfileByAdminCommand, ProfileViewModel>,
                UpdateProfileByAdminPreProcessor
            >()
            .AddScoped<
                IPipelineBehavior<UpdateProfileByAdminCommand, ProfileViewModel>,
                UpdateProfileByAdminPostProcessor
            >();

        var appSettings = new AppSettings();

        builder.Configuration.Bind(appSettings);

        services.AddSingleton(appSettings);

        services.AddRateLimiting();

        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = builder.Environment.IsDevelopment();
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        services.AddGrpcHealthChecks();

        services.AddSingleton(_ =>
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(StringTrimmerJsonConverter.Instance);
            options.Converters.Add(DecimalJsonConverter.Instance);
            return options;
        });

        builder.AddPersistenceServices();

        services.AddValidatorsFromAssemblyContaining<IPersonaApiMarker>(includeInternalTypes: true);

        services.AddTransient(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);

        services.AddSingleton<IActivityScope, ActivityScope>();
        services.AddSingleton<CommandHandlerMetrics>();
        services.AddSingleton<QueryHandlerMetrics>();

        services.AddVersioning();
        services.AddEndpoints(typeof(IPersonaApiMarker));

        services.AddMapper(typeof(IPersonaApiMarker));

        services.AddScoped<KeycloakTokenIntrospectionMiddleware>();
    }
}
