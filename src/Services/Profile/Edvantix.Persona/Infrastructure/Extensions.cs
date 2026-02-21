using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<PersonaDbContext>(
            Components.Database.Persona,
            app =>
            {
                if (app.Environment.IsDevelopment())
                {
                    services.AddMigration<PersonaDbContext>();
                }
                else
                {
                    services.AddMigration<PersonaDbContext>();
                }

                services.AddRepositories(typeof(IPersonaApiMarker));
            }
        );

        builder.AddAzureBlobStorage();
    }
}
