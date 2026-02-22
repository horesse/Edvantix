using Edvantix.Persona.Infrastructure.Blob;
using Edvantix.Persona.Infrastructure.Repositories;

namespace Edvantix.Persona.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<PersonaDbContext>(
            Components.Database.Persona,
            _ => services.AddMigration<PersonaDbContext>()
        );

        // Автономный репозиторий регистрируется вручную (не через AddRepositories,
        // так как IProfileRepository не наследует IRepository<T>)
        services.AddScoped<IProfileRepository, ProfileRepository>();

        builder.AddAzureBlobStorage();
    }
}
