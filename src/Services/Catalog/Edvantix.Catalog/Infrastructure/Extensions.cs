using Edvantix.Catalog.Infrastructure.DistributedLock;
using Edvantix.Catalog.Infrastructure.Idempotency;
using Edvantix.Catalog.Infrastructure.Seeders;
using Edvantix.Chassis.Utilities.Configurations;

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
                services.AddMigration<CatalogDbContext, CatalogDbSeeder>();
                services.AddRepositories(typeof(ICatalogApiMarker));
            }
        );
    }

    /// <summary>
    /// Регистрирует Redis-клиент, HybridCache, distributed lock и idempotency manager.
    /// </summary>
    public static void AddRedisInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        // Configure Redis
        builder
            .AddRedisClientBuilder(Components.Redis, o => o.DisableAutoActivation = false)
            .WithAzureAuthentication();

        builder.Configure<CachingOptions>(CachingOptions.ConfigurationSection);

        var cachingOptions = services.BuildServiceProvider().GetRequiredService<CachingOptions>();

        services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = cachingOptions.MaximumPayloadBytes;

            options.DefaultEntryOptions = new()
            {
                Expiration = cachingOptions.Expiration,
                LocalCacheExpiration = cachingOptions.Expiration,
            };
        });

        builder.AddDistributedLock();

        services.AddSingleton<IRequestManager, RequestManager>();
    }
}
