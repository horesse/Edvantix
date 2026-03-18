namespace Edvantix.Organizations.Infrastructure;

/// <summary>
/// Infrastructure registration extensions for the Organizations service.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers the Organizations DbContext, runs migrations on startup, and wires repositories.
    /// </summary>
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<OrganizationsDbContext>(
            Components.Database.Organizations,
            _ =>
            {
                services.AddMigration<OrganizationsDbContext>();
                services.AddRepositories(typeof(IOrganizationsApiMarker));
            }
        );
    }
}
