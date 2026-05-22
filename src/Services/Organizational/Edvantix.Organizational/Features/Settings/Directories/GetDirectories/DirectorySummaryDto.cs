namespace Edvantix.Organizational.Features.Settings.Directories.GetDirectories;

/// <summary>
/// Сводная карточка справочника для отображения в каталоге настроек.
/// Объединяет метаданные из <see cref="DirectoryCatalog"/> со статистикой из <see cref="DirectoryStats"/>.
/// </summary>
/// <param name="Code">Стабильный машинный код справочника (kebab-case), напр. <c>levels</c>.</param>
/// <param name="Name">Локализованное отображаемое имя.</param>
/// <param name="Description">Краткое описание для карточки UI.</param>
/// <param name="Icon">Имя иконки Lucide.</param>
/// <param name="Badge">Опциональный системный бейдж.</param>
/// <param name="ActiveCount">Количество не архивных записей.</param>
/// <param name="ArchivedCount">Количество архивных записей.</param>
/// <param name="LastModifiedAt">Время последнего изменения любой записи справочника.</param>
/// <param name="IsAvailable">Доступен ли справочник для редактирования.</param>
public sealed record DirectorySummaryDto(
    string Code,
    string Name,
    string Description,
    string Icon,
    string? Badge,
    int ActiveCount,
    int ArchivedCount,
    DateTimeOffset? LastModifiedAt,
    bool IsAvailable
)
{
    /// <summary>Собирает DTO из дескриптора и статистики справочника.</summary>
    /// <param name="descriptor">Метаданные справочника из <see cref="DirectoryCatalog"/>.</param>
    /// <param name="stats">Статистика из <see cref="IDirectoryStatsProvider"/>.</param>
    public static DirectorySummaryDto From(DirectoryDescriptor descriptor, DirectoryStats stats) =>
        new(
            descriptor.Code,
            descriptor.Name,
            descriptor.Description,
            descriptor.Icon,
            descriptor.Badge,
            stats.ActiveCount,
            stats.ArchivedCount,
            stats.LastModifiedAt,
            stats.IsAvailable
        );
}
