namespace Edvantix.Organizational.Features.Directories.Subjects;

/// <summary>Полное DTO предмета для GET-by-id, включая описание и аудит-поля.</summary>
/// <param name="Id">Идентификатор предмета.</param>
/// <param name="Name">Отображаемое название.</param>
/// <param name="Code">Уникальный код.</param>
/// <param name="Color">Цвет в формате <c>#RRGGBB</c>.</param>
/// <param name="Description">Описание предмета.</param>
/// <param name="Order">Порядок сортировки в UI.</param>
/// <param name="IsArchived">Признак архивной записи.</param>
/// <param name="CreatedAt">Дата и время создания.</param>
/// <param name="LastModified">Дата и время последнего изменения.</param>
public sealed record SubjectDto(
    Guid Id,
    string Name,
    string Code,
    string Color,
    string? Description,
    int Order,
    bool IsArchived,
    DateTime CreatedAt,
    DateTime? LastModified
);
