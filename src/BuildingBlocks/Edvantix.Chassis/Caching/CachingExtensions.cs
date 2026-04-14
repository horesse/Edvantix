using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edvantix.Chassis.Caching;

public static class CachingExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует сервисы гибридного кэширования в контейнере зависимостей,
        /// настраивая параметры из <see cref="CachingOptions" /> и опционального делегата.
        /// </summary>
        /// <param name="configure">
        /// Опциональный делегат для дополнительной настройки <see cref="HybridCacheOptions" />
        /// после применения значений по умолчанию из <see cref="CachingOptions" />.
        /// </param>
        public void AddCaching(Action<HybridCacheOptions>? configure = null)
        {
            var services = builder.Services;

            builder.Configure<CachingOptions>(CachingOptions.ConfigurationSection);

            var cachingOptions = services
                .BuildServiceProvider()
                .GetRequiredService<CachingOptions>();

            services.AddHybridCache(options =>
            {
                options.MaximumPayloadBytes = cachingOptions.MaximumPayloadBytes;

                options.DefaultEntryOptions = new()
                {
                    Expiration = cachingOptions.Expiration,
                    LocalCacheExpiration = cachingOptions.Expiration,
                };

                configure?.Invoke(options);
            });

            services.AddSingleton<IHybridCache, HybridCacheService>();
        }
    }
}
