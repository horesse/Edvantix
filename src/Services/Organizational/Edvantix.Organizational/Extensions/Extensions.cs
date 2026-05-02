using System.Text.Json;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Organizational.Configurations;
using Edvantix.Organizational.Grpc;
using Edvantix.Organizational.IntegrationEvents;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Edvantix.ServiceDefaults.Cors;
using Wolverine.EntityFrameworkCore;
using Wolverine.Persistence;
using Wolverine.Postgresql;

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

        services.AddEventBus(
            typeof(IOrganizationalApiMarker),
            options =>
            {
                var connectionString = builder.Configuration.GetRequiredConnectionString(
                    Components.Database.Organizational
                );

                options.PersistMessagesWithPostgresql(connectionString);
                options.UseEntityFrameworkCoreTransactions(TransactionMiddlewareMode.Lightweight);

                options.Policies.AutoApplyTransactions();

                options.AddEvents();
            }
        );

        builder.AddGrpcServices();
    }
}
