using System.ComponentModel.DataAnnotations;
using Edvantix.SharedKernel.Helpers;

namespace Edvantix.Catalog.Infrastructure.Idempotency;

/// <summary>
/// Запись об обработанной команде для предотвращения дублирования.
/// Хранится в Redis с TTL, равным окну идемпотентности.
/// </summary>
public sealed class ClientRequest
{
    /// <summary>Ключ идемпотентности, переданный клиентом.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Полное имя типа команды для диагностики.</summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>Момент первичной обработки запроса (UTC).</summary>
    public DateTime Time { get; set; } = DateTimeHelper.UtcNow();
}
