namespace Edvantix.Catalog.Infrastructure.DistributedLock;

/// <summary>
/// Представляет захваченную блокировку с поддержкой детерминированного освобождения.
/// Всегда используйте в блоке using или await using для гарантированного освобождения.
/// </summary>
public interface IDistributedAccessLock : IDisposable, IAsyncDisposable
{
    /// <summary>Признак успешного захвата блокировки.</summary>
    bool IsAcquired { get; }

    /// <summary>Исключение, возникшее при попытке захвата (null если захват успешен).</summary>
    Exception? Exception { get; }
}
