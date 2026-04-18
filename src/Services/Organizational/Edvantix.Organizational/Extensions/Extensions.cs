using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Organizational.Configurations;
using Edvantix.Organizational.CQRS.Pipelines;
using Edvantix.Organizational.Grpc;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Edvantix.ServiceDefaults.Cors;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Organizational.Extensions;

internal static class MediatorExtensions
{
    /// <summary>
    /// Регистрирует <see cref="AuthorizationBehavior{TMessage,TResponse}"/> в конвейере Mediator.
    /// Проверяет разрешения профиля в организации для команд и запросов с атрибутом <c>[RequirePermission]</c>.
    /// </summary>
    internal static IServiceCollection ApplyAuthorizationBehavior(
        this IServiceCollection services
    )
    {
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(AuthorizationBehavior<,>)
        );
        return services;
    }
}

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        builder.AddAppSettings<OrganizationalAppSettings>();

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
                            $"{Services.Organisational}_{Authorization.Actions.Read}",
                            $"{Services.Organisational}_{Authorization.Actions.Write}"
                        );
                }
            )
            .SetDefaultPolicy(
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireScope($"{Services.Organisational}_{Authorization.Actions.Read}")
                    .Build()
            );

        services.AddTenantContext();

        services.AddValidationExceptionHandler();
        services.AddNotFoundExceptionHandler();
        services.AddForbiddenExceptionHandler();
        services.AddGlobalExceptionHandler();
        services.AddProblemDetails();

        builder.AddRateLimiting();

        services
            .AddMediator(
                (MediatorOptions options) => options.ServiceLifetime = ServiceLifetime.Scoped
            )
            .ApplyActivityBehavior()
            .ApplyLoggingBehavior()
            .ApplyValidationBehavior()
            .ApplyAuthorizationBehavior()
            .ApplyTransactionBehavior<OrganizationalDbContext>();

        services.AddRateLimiter();

        services.AddActivityScope().AddCommandHandlerMetrics().AddQueryHandlerMetrics();

        builder.AddGrpcServices();

        services.AddValidatorsFromAssemblyContaining<IOrganizationalApiMarker>(
            includeInternalTypes: true
        );

        services.AddVersioning();
        services.AddEndpoints(typeof(IOrganizationalApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<
                OpenApiInfoDefinitionsTransformer<OrganizationalAppSettings>
            >()
        );

        services.AddMapper(typeof(IOrganizationalApiMarker));

        services.AddTransient(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);

        builder.AddPersistenceServices();

        services.AddKeycloakTokenIntrospection();

        builder.AddEventBus(
            typeof(IOrganizationalApiMarker),
            cfg =>
            {
                cfg.AddEntityFrameworkOutbox<OrganizationalDbContext>(o =>
                {
                    o.QueryDelay = TimeSpan.FromSeconds(1);

                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);

                    o.UsePostgres();

                    o.UseBusOutbox();
                });

                cfg.AddConfigureEndpointsCallback(
                    (context, _, configurator) =>
                        configurator.UseEntityFrameworkOutbox<OrganizationalDbContext>(context)
                );
            }
        );
    }
}
