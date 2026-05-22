namespace Edvantix.Groups.Features.Directories.LessonTypes.Update;

/// <summary>
/// HTTP-тело запроса на обновление типа занятия.
/// </summary>
/// <param name="Name">Новое отображаемое имя.</param>
/// <param name="Code">Новый уникальный код в рамках организации.</param>
/// <param name="DefaultDurationMinutes">Новая длительность по умолчанию (5–600 минут).</param>
/// <param name="Color">Новый цвет в формате HEX (#RRGGBB).</param>
/// <param name="Icon">Новое имя иконки из kit (опционально).</param>
/// <param name="Order">Новый порядок сортировки.</param>
public sealed record UpdateLessonTypeRequest(
    string Name,
    string Code,
    int DefaultDurationMinutes,
    string Color,
    string? Icon,
    int Order = 0
);
