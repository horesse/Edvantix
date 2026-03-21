namespace Edvantix.Scheduling.Infrastructure;

/// <summary>
/// Infrastructure registration extensions for the Scheduling service.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers the Scheduling DbContext, runs migrations on startup, and wires repositories.
    /// </summary>
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<SchedulingDbContext>(
            Components.Database.Scheduling,
            _ =>
            {
                services.AddMigration<SchedulingDbContext>();
                services.AddRepositories(typeof(ISchedulingApiMarker));
            }
        );
    }
}
