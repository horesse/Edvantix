namespace Edvantix.Catalog.Infrastructure.DistributedLock;

/// <summary>
/// Провайдер для захвата распределённых блокировок по ключу ресурса.
/// Используется в командных обработчиках при модификации справочных данных.
/// </summary>
public interface IDistributedAccessLockProvider
{
    /// <summary>
    /// Синхронно пытается захватить блокировку для указанного ресурса.
    /// </summary>
    /// <param name="resourceKey">Уникальный ключ защищаемого ресурса.</param>
    /// <param name="acquireTimeout">Максимальное время ожидания захвата.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    IDistributedAccessLock TryAcquire(
        string resourceKey,
        TimeSpan acquireTimeout,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Асинхронно пытается захватить блокировку для указанного ресурса.
    /// </summary>
    /// <param name="resourceKey">Уникальный ключ защищаемого ресурса.</param>
    /// <param name="acquireTimeout">Максимальное время ожидания захвата.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IDistributedAccessLock> TryAcquireAsync(
        string resourceKey,
        TimeSpan acquireTimeout,
        CancellationToken cancellationToken = default
    );
}
