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
                    services.AddMigration<OrganizationalDbContext>();
                }
                else
                {
                    services.AddMigration<OrganizationalDbContext>();
                }

                services.AddRepositories(typeof(IOrganizationalApiMarker));
            }
        );
    }
}
