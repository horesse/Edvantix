using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Edvantix.Chassis.Caching;

public static class CachingExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует FusionCache (L1 + L2 + Backplane) в контейнере внедрения зависимостей,
        /// повторно используя зарегистрированный Aspire Redis-клиент <see cref="IConnectionMultiplexer" />
        /// как для распределённого кеша, так и для Redis Backplane.
        /// </summary>
        /// <param name="configure">
        /// Необязательный делегат для дополнительной настройки <see cref="IFusionCacheBuilder" />
        /// после применения параметров по умолчанию из <see cref="CachingOptions" />.
        /// </param>
        public void AddCaching(Action<IFusionCacheBuilder>? configure = null)
        {
            var services = builder.Services;
            var provider = services.BuildServiceProvider();

            builder.Configure<CachingOptions>(CachingOptions.ConfigurationSection);

            var cachingOptions = provider.GetRequiredService<CachingOptions>();

            var fusionBuilder = services
                .AddFusionCache()
                .WithDefaultEntryOptions(
                    new FusionCacheEntryOptions
                    {
                        Duration = cachingOptions.Expiration,
                        DistributedCacheDuration = cachingOptions.Expiration,
                    }
                )
                .WithSerializer(new FusionCacheSystemTextJsonSerializer());

            var multiplexer = provider.GetService<IConnectionMultiplexer>();

            if (multiplexer is not null)
            {
                fusionBuilder.WithDistributedCache(sp => new RedisCache(
                    new RedisCacheOptions
                    {
                        ConnectionMultiplexerFactory = () =>
                            Task.FromResult(sp.GetRequiredService<IConnectionMultiplexer>()),
                    }
                ));
            }

            services
                .AddOpenTelemetry()
                .WithMetrics(metrics => metrics.AddFusionCacheInstrumentation())
                .WithTracing(tracing => tracing.AddFusionCacheInstrumentation());

            configure?.Invoke(fusionBuilder);
        }
    }
}
