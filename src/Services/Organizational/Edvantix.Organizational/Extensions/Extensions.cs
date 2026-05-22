using System.Text.Json;
using Edvantix.Chassis.EventBus.Wolverine;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Organizational.Configurations;
using Edvantix.Organizational.Grpc;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Edvantix.ServiceDefaults.Cors;

namespace Edvantix.Organizational.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        builder.AddAppSettings<OrganizationalAppSettings>();

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
        services.AddEndpoints(typeof(IOrganizationalApiMarker));
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<
                OpenApiInfoDefinitionsTransformer<OrganizationalAppSettings>
            >()
        );

        services.AddMapper(typeof(IOrganizationalApiMarker));

        // Регистрируем провайдеры статистики справочников (IDirectoryStatsProvider) и
        // чекеры уникальности имён (IUniqueNameChecker) через Scrutor-сканирование.
        services.Scan(scan =>
            scan.FromAssembliesOf(typeof(IOrganizationalApiMarker))
                .AddClasses(
                    classes =>
                        classes.AssignableTo(
                            typeof(Edvantix.Organizational.Features.Settings.Directories.IDirectoryStatsProvider)
                        ),
                    false
                )
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.Scan(scan =>
            scan.FromAssembliesOf(typeof(IOrganizationalApiMarker))
                .AddClasses(
                    classes =>
                        classes.AssignableTo(
                            typeof(Edvantix.Organizational.Features.Settings.Directories.IUniqueNameChecker)
                        ),
                    false
                )
                .AsSelf()
                .WithScopedLifetime()
        );

        builder.AddEventBus(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(IOrganizationalApiMarker).Assembly);
            opts.ListenToIntegrationEventsIn(typeof(IOrganizationalApiMarker).Assembly);
        });

        builder.AddGrpcServices();
    }
}
