using System.Text.Json;
using Edvantix.Blog.Configurations;
using Edvantix.Blog.Grpc;
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
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Microsoft.AspNetCore.Authorization;
using AspireServices = Edvantix.Constants.Aspire.Services;

namespace Edvantix.Blog.Extensions;

/// <summary>
/// Регистрация всех сервисов микросервиса Blog.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Регистрирует CQRS, persistence, gRPC-клиенты, авторизацию и все feature-сервисы.
    /// </summary>
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        builder.AddAppSettings<BlogAppSettings>();

        builder.AddDefaultAuthentication().WithKeycloakClaimsTransformation();

        // Административные операции требуют роли admin и соответствующих scopes
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
                            $"{AspireServices.Blog}_{Authorization.Actions.Read}",
                            $"{AspireServices.Blog}_{Authorization.Actions.Write}"
                        );
                }
            )
            .SetDefaultPolicy(
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireScope($"{AspireServices.Blog}_{Authorization.Actions.Read}")
                    .Build()
            );

        // Обработчики ошибок в порядке специфичности (сначала более специфичные)
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<ForbiddenExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services
            .AddMediator(
                (MediatorOptions options) => options.ServiceLifetime = ServiceLifetime.Scoped
            )
            .ApplyActivityBehavior()
            .ApplyLoggingBehavior()
            .ApplyValidationBehavior();

        builder.AddRateLimiting();

        services.AddSingleton(_ =>
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(StringTrimmerJsonConverter.Instance);
            options.Converters.Add(DecimalJsonConverter.Instance);
            return options;
        });

        builder.AddPersistenceServices();

        services.AddValidatorsFromAssemblyContaining<IBlogApiMarker>(includeInternalTypes: true);

        services.AddTransient(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);

        services.AddActivityScope().AddCommandHandlerMetrics().AddQueryHandlerMetrics();

        services.AddVersioning();
        services.AddEndpoints(typeof(IBlogApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<OpenApiInfoDefinitionsTransformer<BlogAppSettings>>()
        );

        services.AddMapper(typeof(IBlogApiMarker));

        services.AddKeycloakTokenIntrospection();
        
        builder.AddGrpcServices();
    }
}
