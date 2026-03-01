using System.Text.Json;
using Edvantix.Catalog.Configurations;
using Edvantix.Chassis.CQRS.Command;
using Edvantix.Chassis.CQRS.Pipelines;
using Edvantix.Chassis.CQRS.Query;
using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Microsoft.AspNetCore.Authorization;
using AspireServices = Edvantix.Constants.Aspire.Services;

namespace Edvantix.Catalog.Extensions;

/// <summary>
/// Регистрация всех сервисов микросервиса Catalog.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Регистрирует CQRS, persistence, авторизацию, кеширование и все feature-сервисы.
    /// </summary>
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        builder.AddAppSettings<CatalogAppSettings>();

        builder.AddDefaultAuthentication().WithKeycloakClaimsTransformation();

        // Операции записи в справочники требуют роли admin
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
                            $"{AspireServices.Catalog}_{Authorization.Actions.Read}",
                            $"{AspireServices.Catalog}_{Authorization.Actions.Write}"
                        );
                }
            )
            .SetDefaultPolicy(
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireScope($"{AspireServices.Catalog}_{Authorization.Actions.Read}")
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
            // Open-generic behaviors run first (activity tracing → logging → validation)
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ActivityBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        builder.AddRateLimiting();

        services.AddSingleton(_ =>
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(StringTrimmerJsonConverter.Instance);
            return options;
        });

        builder.AddPersistenceServices();

        builder.AddRedisInfrastructure();

        services.AddValidatorsFromAssemblyContaining<ICatalogApiMarker>(includeInternalTypes: true);

        services.AddTransient(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);

        services.AddSingleton<IActivityScope, ActivityScope>();
        services.AddSingleton<CommandHandlerMetrics>();
        services.AddSingleton<QueryHandlerMetrics>();

        services.AddVersioning();
        services.AddEndpoints(typeof(ICatalogApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<OpenApiInfoDefinitionsTransformer<CatalogAppSettings>>()
        );

        services.AddMapper(typeof(ICatalogApiMarker));

        services.AddScoped<KeycloakTokenIntrospectionMiddleware>();

        // gRPC-сервер для предоставления справочных данных другим сервисам
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = builder.Environment.IsDevelopment();
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        services.AddGrpcHealthChecks();
    }
}
