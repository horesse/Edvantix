namespace Edvantix.Groups.Features.Directories.LessonTypes.List;

/// <summary>
/// Краткое DTO типа занятия для отображения в списке.
/// </summary>
/// <param name="Id">Идентификатор (ключ строки в UI).</param>
/// <param name="Name">Отображаемое имя.</param>
/// <param name="Code">Уникальный код в рамках организации.</param>
/// <param name="DefaultDurationMinutes">Длительность по умолчанию (минуты).</param>
/// <param name="Color">Цвет в формате HEX (#RRGGBB).</param>
/// <param name="Icon">Имя иконки из kit (опционально).</param>
/// <param name="Order">Порядок сортировки в UI.</param>
/// <param name="IsArchived">Признак архивирования.</param>
public sealed record LessonTypeListItemDto(
    Guid Id,
    string Name,
    string Code,
    int DefaultDurationMinutes,
    string Color,
    string? Icon,
    int Order,
    bool IsArchived
);
