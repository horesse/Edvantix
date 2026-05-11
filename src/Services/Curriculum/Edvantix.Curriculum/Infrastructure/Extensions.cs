namespace Edvantix.Curriculum.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<CurriculumDbContext>(
            Components.Database.Curriculum,
            _ =>
            {
                services.AddMigration<CurriculumDbContext>();
                services.AddRepositories(typeof(ICurriculumApiMarker));
            }
        );
    }
}
