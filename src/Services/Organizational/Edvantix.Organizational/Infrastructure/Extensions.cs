namespace Edvantix.Organizational.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<OrganizationalDbContext>(
            Components.Database.Organizational,
            _ =>
            {
                services.AddMigration<OrganizationalDbContext, PermissionsDbSeeder>();
                services.AddRepositories(typeof(IOrganizationalApiMarker));
            }
        );

        builder
            .AddRedisClientBuilder(Components.Redis, o => o.DisableAutoActivation = false)
            .WithAzureAuthentication();

        builder.AddCaching();
    }
}
