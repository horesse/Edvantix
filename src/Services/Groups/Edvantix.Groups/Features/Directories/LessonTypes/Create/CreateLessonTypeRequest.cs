namespace Edvantix.Groups.Features.Directories.LessonTypes.Create;

/// <summary>
/// HTTP-тело запроса на создание типа занятия.
/// </summary>
/// <param name="Name">Отображаемое имя.</param>
/// <param name="Code">Уникальный код в рамках организации (только A–Z, 0–9, _, -).</param>
/// <param name="DefaultDurationMinutes">Длительность по умолчанию (5–600 минут).</param>
/// <param name="Color">Цвет для UI в формате HEX (#RRGGBB).</param>
/// <param name="Icon">Имя иконки из kit (опционально).</param>
/// <param name="Order">Порядок сортировки в UI (по умолчанию 0).</param>
public sealed record CreateLessonTypeRequest(
    string Name,
    string Code,
    int DefaultDurationMinutes,
    string Color,
    string? Icon,
    int Order = 0
);
