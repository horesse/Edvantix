using System.Text.Json;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Curriculum.Configurations;
using Edvantix.Chassis.EventBus.Wolverine;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Edvantix.ServiceDefaults.Cors;

namespace Edvantix.Curriculum.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        builder.AddAppSettings<CurriculumAppSettings>();

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
        services.AddEndpoints(typeof(ICurriculumApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<OpenApiInfoDefinitionsTransformer<CurriculumAppSettings>>()
        );

        services.AddMapper(typeof(ICurriculumApiMarker));

        builder.AddEventBus(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(ICurriculumApiMarker).Assembly);
            opts.ListenToIntegrationEventsIn(typeof(ICurriculumApiMarker).Assembly);
        });
    }
}
