using Edvantix.Chassis.EventBus.Wolverine;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Groups.Configurations;
using Edvantix.Groups.Grpc;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Edvantix.ServiceDefaults.Cors;

namespace Edvantix.Groups.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        builder.AddAppSettings<GroupsAppSettings>();

        builder.AddSecurityServices();

        services.AddMemoryCache();

        services.AddTenantContext();

        services.AddValidationExceptionHandler();
        services.AddNotFoundExceptionHandler();
        services.AddForbiddenExceptionHandler();
        services.AddGlobalExceptionHandler();
        services.AddProblemDetails();

        services.AddCqrsInfrastructure();

        builder.AddGrpcServices();

        builder.AddRateLimiting();

        builder.AddPersistenceServices();

        services.AddVersioning();
        services.AddEndpoints(typeof(IGroupsApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<OpenApiInfoDefinitionsTransformer<GroupsAppSettings>>()
        );

        services.AddMapper(typeof(IGroupsApiMarker));

        builder.AddEventBus(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(IGroupsApiMarker).Assembly);
            opts.ListenToIntegrationEventsIn(typeof(IGroupsApiMarker).Assembly);
        });

    }
}
