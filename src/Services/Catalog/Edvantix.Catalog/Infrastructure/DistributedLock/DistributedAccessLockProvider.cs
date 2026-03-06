using Medallion.Threading;

namespace Edvantix.Catalog.Infrastructure.DistributedLock;

/// <summary>
/// Реализация провайдера распределённых блокировок поверх Medallion.Threading.
/// Делегирует захват блокировок в Redis через IDistributedLockProvider.
/// </summary>
public sealed class DistributedAccessLockProvider(IDistributedLockProvider synchronizationProvider)
    : IDistributedAccessLockProvider
{
    /// <inheritdoc />
    public IDistributedAccessLock TryAcquire(
        string resourceKey,
        TimeSpan acquireTimeout,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var handle = synchronizationProvider.AcquireLock(
                resourceKey,
                acquireTimeout,
                cancellationToken
            );

            return new DisposableDistributedAccessLock<IDistributedSynchronizationHandle>(
                true,
                handle
            );
        }
        catch (Exception ex)
        {
            return new DisposableDistributedAccessLock<IDistributedSynchronizationHandle>(ex);
        }
    }

    /// <inheritdoc />
    public async Task<IDistributedAccessLock> TryAcquireAsync(
        string resourceKey,
        TimeSpan acquireTimeout,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var handle = await synchronizationProvider.AcquireLockAsync(
                resourceKey,
                acquireTimeout,
                cancellationToken
            );

            return new DisposableDistributedAccessLock<IDistributedSynchronizationHandle>(
                true,
                handle
            );
        }
        catch (Exception ex)
        {
            return new DisposableDistributedAccessLock<IDistributedSynchronizationHandle>(ex);
        }
    }
}
