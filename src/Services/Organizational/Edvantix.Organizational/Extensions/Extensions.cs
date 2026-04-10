using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.Mapper;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Organizational.Configurations;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Edvantix.ServiceDefaults.Cors;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Organizational.Extensions;

internal static class Extensions
{
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
                    .RequireScope($"{Services.Organizational}_{Authorization.Actions.Read}")
                    .Build()
            );

        services.AddTenantContext();

        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        builder.AddAppSettings<OrganizationalAppSettings>();

        builder.AddRateLimiting();

        services
            .AddMediator(
                (MediatorOptions options) => options.ServiceLifetime = ServiceLifetime.Scoped
            )
            .ApplyActivityBehavior()
            .ApplyLoggingBehavior()
            .ApplyValidationBehavior();

        services.AddActivityScope().AddCommandHandlerMetrics().AddQueryHandlerMetrics();

        services.AddVersioning();
        services.AddEndpoints(typeof(IOrganizationalApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<
                OpenApiInfoDefinitionsTransformer<OrganizationalAppSettings>
            >()
        );

        services.AddMapper(typeof(IOrganizationalApiMarker));

        services.AddValidatorsFromAssemblyContaining<IOrganizationalApiMarker>(
            includeInternalTypes: true
        );

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
