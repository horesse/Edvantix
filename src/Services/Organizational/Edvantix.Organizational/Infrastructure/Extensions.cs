using Edvantix.Organizational.Infrastructure.PermissionModules;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        // Регистрация модулей разрешений: сидер обнаруживает их через DI.
        services.AddSingleton<PermissionModule, OrganizationPermissionModule>();
        services.AddSingleton<PermissionModule, LevelPermissionModule>();
        services.AddSingleton<PermissionModule, SubjectPermissionModule>();
        services.AddSingleton<PermissionModule, LessonTypePermissionModule>();

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
