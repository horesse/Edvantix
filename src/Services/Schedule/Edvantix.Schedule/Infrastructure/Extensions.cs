namespace Edvantix.Schedule.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<ScheduleDbContext>(
            Components.Database.Schedule,
            _ =>
            {
                services.AddMigration<ScheduleDbContext>();
                services.AddRepositories(typeof(IScheduleApiMarker));
            }
        );
    }
}
