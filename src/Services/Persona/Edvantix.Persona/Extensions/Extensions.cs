using System.Text.Json;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Persona.Configurations;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Wolverine.EntityFrameworkCore;
using Wolverine.Persistence;
using Wolverine.Postgresql;

namespace Edvantix.Persona.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();
        
        builder.AddAppSettings<PersonaAppSettings>();

        builder.AddSecurityServices();
        
        // Add exception handlers
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddCqrsInfrastructure();
        
        builder.AddRateLimiting();

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
            options.Converters.Add(DateOnlyJsonConverter.Instance);
            return options;
        });

        builder.AddPersistenceServices();

        services.AddValidatorsFromAssemblyContaining<IPersonaApiMarker>(includeInternalTypes: true);

        services.AddTransient(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);
        
        services.AddVersioning();
        services.AddEndpoints(typeof(IPersonaApiMarker));
        
        services.AddDefaultOpenApi(options =>
            options.AddDocumentTransformer<OpenApiInfoDefinitionsTransformer<PersonaAppSettings>>()
        );

        services.AddMapper(typeof(IPersonaApiMarker));

        builder.AddEventBus(
            typeof(IPersonaApiMarker),
            options =>
            {
                var connectionString = builder.Configuration.GetRequiredConnectionString(
                    Components.Database.Catalog
                );

                options.PersistMessagesWithPostgresql(connectionString);

                options.UseEntityFrameworkCoreTransactions(TransactionMiddlewareMode.Lightweight);

                options.Policies.AutoApplyTransactions();
            }
        );
    }
}
