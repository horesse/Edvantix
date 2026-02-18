using System.Text.Json;
using Edvantix.Blog.Features;
using Edvantix.Blog.Grpc;
using Edvantix.Blog.Infrastructure;
using Edvantix.Chassis.Converter;
using Edvantix.Chassis.CQRS.Command;
using Edvantix.Chassis.CQRS.Pipelines;
using Edvantix.Chassis.CQRS.Query;
using Edvantix.Chassis.Endpoints;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Constants.Core;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi;
using Edvantix.ServiceDefaults.Kestrel;
using FluentValidation;
using MediatR;
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

        builder.AddDefaultOpenApi();

        // Обработчики ошибок в порядке специфичности (сначала более специфичные)
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<ForbiddenExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddApiFeature();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(IBlogApiMarker).Assembly);

            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ActivityBehavior<,>));
        });

        var appSettings = new AppSettings();
        builder.Configuration.Bind(appSettings);
        services.AddSingleton(appSettings);

        services.AddRateLimiting();

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

        services.AddSingleton<IActivityScope, ActivityScope>();
        services.AddSingleton<CommandHandlerMetrics>();
        services.AddSingleton<QueryHandlerMetrics>();

        services.AddVersioning();
        services.AddEndpoints(typeof(IBlogApiMarker));

        services.AddConverter(typeof(IBlogApiMarker));

        services.AddScoped<KeycloakTokenIntrospectionMiddleware>();

        builder.AddGrpcServices();
    }
}
