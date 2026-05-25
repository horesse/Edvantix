namespace Edvantix.Organizational.Features.Directories.LessonTypes;

/// <summary>
/// Полное DTO типа занятия (для GET /{id}).
/// </summary>
/// <param name="Id">Идентификатор.</param>
/// <param name="Name">Отображаемое имя.</param>
/// <param name="Code">Уникальный код в рамках организации.</param>
/// <param name="DefaultDurationMinutes">Длительность по умолчанию (минуты).</param>
/// <param name="Color">Цвет в формате HEX (#RRGGBB).</param>
/// <param name="Icon">Имя иконки из kit (опционально).</param>
/// <param name="Order">Порядок сортировки в UI.</param>
/// <param name="IsArchived">Признак архивирования.</param>
/// <param name="CreatedAt">Дата создания.</param>
/// <param name="LastModifiedAt">Дата последнего изменения.</param>
public sealed record LessonTypeDto(
    Guid Id,
    string Name,
    string Code,
    int DefaultDurationMinutes,
    string Color,
    string? Icon,
    int Order,
    bool IsArchived,
    DateTime CreatedAt,
    DateTime? LastModifiedAt
);
