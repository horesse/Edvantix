using System.Text.Json;
using Edvantix.Chassis.EventBus.Wolverine;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Schedule.Configurations;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Edvantix.ServiceDefaults.Cors;

namespace Edvantix.Schedule.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        builder.AddAppSettings<ScheduleAppSettings>();

        builder.AddSecurityServices();

        services.AddTenantContext();

        services.AddValidationExceptionHandler();
        services.AddNotFoundExceptionHandler();
        services.AddForbiddenExceptionHandler();
        services.AddGlobalExceptionHandler();
        services.AddProblemDetails();

        services.AddCqrsInfrastructure();

        services.AddSingleton(
            new JsonSerializerOptions { Converters = { DateOnlyJsonConverter.Instance } }
        );

        builder.AddRateLimiting();

        builder.AddPersistenceServices();

        services.AddVersioning();
        services.AddEndpoints(typeof(IScheduleApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<OpenApiInfoDefinitionsTransformer<ScheduleAppSettings>>()
        );

        services.AddMapper(typeof(IScheduleApiMarker));

        builder.AddEventBus(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(IScheduleApiMarker).Assembly);
            opts.ListenToIntegrationEventsIn(typeof(IScheduleApiMarker).Assembly);
        });
    }
}
