namespace Edvantix.Organizational.Features.Settings.Directories;

/// <summary>
/// Поставщик статистики по конкретному справочнику.
/// <para>Каждый справочник из <see cref="DirectoryCatalog"/> регистрирует свою реализацию,
/// которая возвращает агрегированные данные для отображения в каталоге настроек.</para>
/// </summary>
public interface IDirectoryStatsProvider
{
    /// <summary>Метаданные справочника, обслуживаемого этим провайдером.</summary>
    DirectoryDescriptor Descriptor { get; }

    /// <summary>Возвращает текущую статистику справочника для указанной организации.</summary>
    /// <param name="orgId">Идентификатор организации (тенанта).</param>
    /// <param name="ct">Токен отмены.</param>
    Task<DirectoryStats> GetStatsAsync(Guid orgId, CancellationToken ct);
}
