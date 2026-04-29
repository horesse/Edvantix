namespace Edvantix.Audit.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<AuditDbContext>(
            Components.Database.Audit,
            _ =>
            {
                services.AddMigration<AuditDbContext>();
                services.AddRepositories(typeof(IAuditApiMarker));
            }
        );

        builder
            .AddRedisClientBuilder(Components.Redis, o => o.DisableAutoActivation = false)
            .WithAzureAuthentication();

        builder.AddCaching();
    }
}
