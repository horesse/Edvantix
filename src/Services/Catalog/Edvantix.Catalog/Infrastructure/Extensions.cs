namespace Edvantix.Catalog.Infrastructure;

/// <summary>
/// Расширения для регистрации инфраструктурных сервисов микросервиса Catalog.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Регистрирует CatalogDbContext, запускает миграции и регистрирует все репозитории из сборки.
    /// </summary>
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<CatalogDbContext>(
            Components.Database.Catalog,
            app =>
            {
                services.AddMigration<CatalogDbContext>();
                services.AddRepositories(typeof(ICatalogApiMarker));
            }
        );
    }
}
