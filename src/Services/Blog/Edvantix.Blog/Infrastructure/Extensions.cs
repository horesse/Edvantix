using Edvantix.Chassis.EF;
using Edvantix.Chassis.Repository;
using Edvantix.Constants.Aspire;

namespace Edvantix.Blog.Infrastructure;

/// <summary>
/// Расширения для регистрации инфраструктурных сервисов микросервиса Blog.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Регистрирует BlogContext, запускает миграции и регистрирует все репозитории из сборки.
    /// </summary>
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<BlogContext>(
            Components.Database.Blog,
            app =>
            {
                services.AddMigration<BlogContext>();
                services.AddRepositories(typeof(IBlogApiMarker));
            }
        );
    }
}
