namespace Edvantix.Blog.Infrastructure;

/// <summary>
/// Расширения для регистрации инфраструктурных сервисов микросервиса Blog.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Регистрирует BlogDbContext, запускает миграции и регистрирует все репозитории из сборки.
    /// </summary>
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<BlogDbContext>(
            Components.Database.Blog,
            app =>
            {
                services.AddMigration<BlogDbContext>();
                services.AddRepositories(typeof(IBlogApiMarker));
            }
        );
    }
}
