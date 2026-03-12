using Edvantix.Persona.Infrastructure.Blob;
using Edvantix.Persona.Infrastructure.Keycloak;

namespace Edvantix.Persona.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<PersonaDbContext>(
            Components.Database.Persona,
            _ =>
            {
                services.AddMigration<PersonaDbContext>();

                services.AddRepositories(typeof(IPersonaApiMarker));
            }
        );

        builder.AddAzureBlobStorage();

        services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();

        if (builder.Environment.IsDevelopment())
            services.AddHostedService<KeycloakProfileSyncService>();
    }
}
