using System.Text.Json;
using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.CQRS.Command;
using Edvantix.Chassis.CQRS.Pipelines;
using Edvantix.Chassis.CQRS.Query;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Organizational.Configurations;
using Edvantix.Organizational.Features;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Microsoft.AspNetCore.Authorization;
using AspireServices = Edvantix.Constants.Aspire.Services;

namespace Edvantix.Organizational.Extensions;

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
                            $"{AspireServices.Organizational}_{Authorization.Actions.Read}",
                            $"{AspireServices.Organizational}_{Authorization.Actions.Write}"
                        );
                }
            )
            .AddPolicy(
                Authorization.Policies.ProfileRequired,
                policy => policy.RequireAuthenticatedUser().RequireProfileRegistered()
            )
            .SetDefaultPolicy(
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireScope($"{AspireServices.Organizational}_{Authorization.Actions.Read}")
                    .RequireProfileRegistered()
                    .Build()
            );

        // Регистрируем обработчик ProfileRegisteredRequirement и кастомный
        // IAuthorizationMiddlewareResultHandler для возврата PROFILE_NOT_REGISTERED.
        services.AddProfileRequiredServices();

        // Add exception handlers
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<ForbiddenExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddApiFeature();

        services
            .AddMediator(
                (MediatorOptions options) => options.ServiceLifetime = ServiceLifetime.Scoped
            )
            .ApplyActivityBehavior()
            .ApplyLoggingBehavior()
            .ApplyValidationBehavior();

        builder.AddAppSettings<OrganizationalAppSettings>();

        builder.AddRateLimiting();

        services.AddSingleton(_ =>
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(StringTrimmerJsonConverter.Instance);
            options.Converters.Add(DecimalJsonConverter.Instance);
            return options;
        });

        builder.AddPersistenceServices();

        services.AddValidatorsFromAssemblyContaining<IOrganizationalApiMarker>(
            includeInternalTypes: true
        );

        services.AddTransient(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);

        services.AddActivityScope().AddCommandHandlerMetrics().AddQueryHandlerMetrics();

        services.AddVersioning();
        services.AddEndpoints(typeof(IOrganizationalApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<
                OpenApiInfoDefinitionsTransformer<OrganizationalAppSettings>
            >()
        );

        services.AddMapper(typeof(IOrganizationalApiMarker));

        services.AddKeycloakTokenIntrospection();
    }
}
