using System.Text.Json;
using Edvantix.Audit.Configurations;
using Edvantix.Chassis.EventBus.Wolverine;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Edvantix.ServiceDefaults.Cors;

namespace Edvantix.Audit.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        builder.AddAppSettings<AuditAppSettings>();

        builder.AddSecurityServices();

        services.AddTenantContext();

        services.AddValidationExceptionHandler();
        services.AddNotFoundExceptionHandler();
        services.AddForbiddenExceptionHandler();
        services.AddGlobalExceptionHandler();
        services.AddProblemDetails();

        services.AddCqrsInfrastructure();

        services.AddSingleton(
            new JsonSerializerOptions { Converters = { DecimalJsonConverter.Instance } }
        );

        builder.AddRateLimiting();

        builder.AddPersistenceServices();

        services.AddVersioning();
        services.AddEndpoints(typeof(IAuditApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<OpenApiInfoDefinitionsTransformer<AuditAppSettings>>()
        );

        services.AddMapper(typeof(IAuditApiMarker));

        builder.AddEventBus(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(IAuditApiMarker).Assembly);
            opts.ListenToIntegrationEventsIn(typeof(IAuditApiMarker).Assembly);
        });
    }
}
