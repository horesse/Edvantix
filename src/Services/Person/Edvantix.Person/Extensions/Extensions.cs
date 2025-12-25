using System.Text.Json;
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
using Edvantix.Constants.Aspire;
using Edvantix.Constants.Core;
using Edvantix.Person.CQRS.Pipelines;
using Edvantix.Person.Features;
using Edvantix.Person.Infrastructure;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi;
using Edvantix.ServiceDefaults.Kestrel;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Person.Extensions;

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
                            $"{Services.Person}_{Authorization.Actions.Read}",
                            $"{Services.Person}_{Authorization.Actions.Write}"
                        );
                }
            )
            .SetDefaultPolicy(
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireScope($"{Services.Person}_{Authorization.Actions.Read}")
                    .Build()
            );

        builder.AddDefaultOpenApi();

        // Add exception handlers
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddApiFeature();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(IPersonApiMarker).Assembly);

            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ActivityBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PersonalDataBehavior<,>));
        });

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

        services.AddValidatorsFromAssemblyContaining<IPersonApiMarker>(includeInternalTypes: true);

        services.AddSingleton<IActivityScope, ActivityScope>();
        services.AddSingleton<CommandHandlerMetrics>();
        services.AddSingleton<QueryHandlerMetrics>();

        services.AddVersioning();
        services.AddEndpoints(typeof(IPersonApiMarker));

        services.AddConverter(typeof(IPersonApiMarker));

        services.AddScoped<KeycloakTokenIntrospectionMiddleware>();
    }
}
