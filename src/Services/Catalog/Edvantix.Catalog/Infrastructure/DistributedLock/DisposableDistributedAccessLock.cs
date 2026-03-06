namespace Edvantix.Catalog.Infrastructure.DistributedLock;

/// <summary>
/// Обёртка над захваченной блокировкой с детерминированным освобождением ресурса.
/// Поддерживает оба сценария: успешный захват и ошибку при захвате.
/// </summary>
public sealed class DisposableDistributedAccessLock<TDistributedLock> : IDistributedAccessLock
    where TDistributedLock : IDisposable, IAsyncDisposable
{
    /// <summary>Создаёт экземпляр с успешно захваченной блокировкой.</summary>
    public DisposableDistributedAccessLock(bool isAcquired, TDistributedLock? distributedLock)
    {
        IsAcquired = isAcquired;
        DistributedLock = distributedLock;
        Exception = null;
    }

    /// <summary>Создаёт экземпляр для случая ошибки при захвате блокировки.</summary>
    public DisposableDistributedAccessLock(Exception exception)
    {
        IsAcquired = false;
        DistributedLock = default;
        Exception = exception;
    }

    private TDistributedLock? DistributedLock { get; }

    /// <inheritdoc />
    public bool IsAcquired { get; }

    /// <inheritdoc />
    public Exception? Exception { get; set; }

    /// <inheritdoc />
    public void Dispose() => DistributedLock?.Dispose();

    /// <inheritdoc />
    public ValueTask DisposeAsync() =>
        DistributedLock is not null ? DistributedLock.DisposeAsync() : default;
}
