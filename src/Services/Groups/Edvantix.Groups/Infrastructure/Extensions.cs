namespace Edvantix.Groups.Infrastructure;

/// <summary>
/// Расширения для регистрации инфраструктурных сервисов сервиса групп.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Регистрирует контекст базы данных PostgreSQL и репозитории для сервиса групп.
    /// </summary>
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<GroupsDbContext>(
            Components.Database.Groups,
            _ =>
            {
                services.AddMigration<GroupsDbContext>();
                services.AddRepositories(typeof(IGroupsApiMarker));
            }
        );
    }
}
