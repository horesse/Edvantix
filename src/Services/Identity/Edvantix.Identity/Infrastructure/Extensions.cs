namespace Edvantix.Identity.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<IdentityDbContext>(
            Components.Database.Identity,
            _ =>
            {
                services.AddMigration<IdentityDbContext>();
            },
            excludeDefaultInterceptors: true
        );
    }
}
