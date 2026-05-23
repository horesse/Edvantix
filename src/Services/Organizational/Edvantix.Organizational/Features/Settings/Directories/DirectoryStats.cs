namespace Edvantix.Organizational.Features.Settings.Directories;

/// <summary>
/// Сводная статистика по одному справочнику для одной организации.
/// </summary>
/// <param name="ActiveCount">Количество не архивных записей.</param>
/// <param name="ArchivedCount">Количество архивных записей.</param>
/// <param name="LastModifiedAt">Время последнего изменения любой записи справочника.</param>
/// <param name="IsAvailable">Доступен ли справочник для редактирования (false — заглушка/в разработке).</param>
public sealed record DirectoryStats(
    int ActiveCount,
    int ArchivedCount,
    DateTimeOffset? LastModifiedAt,
    bool IsAvailable
);
