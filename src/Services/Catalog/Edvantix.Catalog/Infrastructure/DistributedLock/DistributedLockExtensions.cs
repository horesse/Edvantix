using Medallion.Threading;
using Medallion.Threading.Redis;
using StackExchange.Redis;

namespace Edvantix.Catalog.Infrastructure.DistributedLock;

/// <summary>
/// Регистрация инфраструктуры распределённых блокировок.
/// </summary>
internal static class DistributedLockExtensions
{
    /// <summary>
    /// Регистрирует Redis-backed distributed lock provider.
    /// Требует зарегистрированного IConnectionMultiplexer (через Aspire Redis интеграцию).
    /// </summary>
    public static void AddDistributedLock(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        // Medallion.Threading.Redis использует IConnectionMultiplexer,
        // который предоставляется Aspire.Microsoft.Azure.StackExchangeRedis
        services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            return new RedisDistributedSynchronizationProvider(multiplexer.GetDatabase());
        });

        services.AddSingleton<IDistributedAccessLockProvider, DistributedAccessLockProvider>();
    }
}
