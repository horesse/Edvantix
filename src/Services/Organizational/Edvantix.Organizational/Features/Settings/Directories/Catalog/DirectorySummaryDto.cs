namespace Edvantix.Organizational.Features.Settings.Directories.Catalog;

/// <summary>
/// Сводная карточка одного справочника для страницы настроек.
/// Объединяет статичные метаданные из <see cref="DirectoryDescriptor"/>
/// и динамическую статистику из <see cref="DirectoryStats"/>.
/// </summary>
/// <param name="Code">Машинный код справочника (kebab-case).</param>
/// <param name="Name">Отображаемое имя справочника.</param>
/// <param name="Description">Краткое описание для карточки в UI.</param>
/// <param name="Icon">Имя иконки Lucide.</param>
/// <param name="Badge">Опциональный бейдж (напр. <c>системный</c>).</param>
/// <param name="ActiveCount">Количество активных (не архивных) записей.</param>
/// <param name="ArchivedCount">Количество архивных записей.</param>
/// <param name="LastModifiedAt">Время последнего изменения любой записи справочника.</param>
/// <param name="IsAvailable">
/// <c>true</c> — справочник доступен для редактирования;
/// <c>false</c> — заглушка или ещё не реализован.
/// </param>
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
);
