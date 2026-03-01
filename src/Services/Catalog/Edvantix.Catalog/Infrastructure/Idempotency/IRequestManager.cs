namespace Edvantix.Catalog.Infrastructure.Idempotency;

/// <summary>
/// Менеджер идемпотентности команд на основе Redis.
/// Позволяет обнаружить повторно отправленные запросы и избежать двойной обработки.
/// </summary>
public interface IRequestManager
{
    /// <summary>
    /// Проверяет, был ли запрос с данным ключом уже обработан.
    /// </summary>
    /// <param name="idempotencyKey">Ключ идемпотентности из заголовка запроса.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<bool> IsExistAsync(string idempotencyKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет запись об обработанном запросе с TTL.
    /// </summary>
    /// <param name="clientRequest">Метаданные обработанной команды.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task CreateAsync(ClientRequest clientRequest, CancellationToken cancellationToken = default);
}
