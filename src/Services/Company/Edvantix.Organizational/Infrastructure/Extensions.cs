using Edvantix.Organizational.Infrastructure.Seeders;

namespace Edvantix.Organizational.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<OrganizationalDbContext>(
            Components.Database.Organizational,
            app =>
            {
                if (app.Environment.IsDevelopment())
                {
                    services.AddMigration<OrganizationalDbContext, OrganizationalDbSeeder>();
                }
                else
                {
                    services.AddMigration<OrganizationalDbContext, OrganizationalDbSeeder>();
                }

                services.AddRepositories(typeof(IOrganizationalApiMarker));
            }
        );
    }
}
