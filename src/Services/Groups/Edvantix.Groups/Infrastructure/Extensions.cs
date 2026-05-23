using Edvantix.Groups.Infrastructure.PermissionModules;
using Edvantix.Permissions;

namespace Edvantix.Groups.Infrastructure;

/// <summary>
/// Расширения для регистрации инфраструктурных сервисов сервиса групп.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Регистрирует контекст базы данных PostgreSQL, репозитории и инфраструктурные сервисы сервиса групп.
    /// </summary>
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        // Регистрация модулей разрешений: сидер подхватывает их через DI и синкает в Organizational.
        services.AddSingleton<PermissionModule, LevelPermissionModule>();
        services.AddSingleton<PermissionModule, GroupPermissionModule>();
        services.AddSingleton<PermissionModule, LessonTypePermissionModule>();

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
