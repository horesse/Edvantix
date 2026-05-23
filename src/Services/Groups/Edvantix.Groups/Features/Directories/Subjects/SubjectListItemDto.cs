namespace Edvantix.Groups.Features.Directories.Subjects;

/// <summary>DTO предмета для списка — содержит только бизнес-поля без аудита.</summary>
/// <param name="Id">Идентификатор предмета.</param>
/// <param name="Name">Отображаемое название.</param>
/// <param name="Code">Уникальный код (напр. <c>MATH</c>).</param>
/// <param name="Color">Цвет в формате <c>#RRGGBB</c>.</param>
/// <param name="Order">Порядок сортировки в UI.</param>
/// <param name="IsArchived">Признак архивной записи.</param>
public sealed record SubjectListItemDto(
    Guid Id,
    string Name,
    string Code,
    string Color,
    int Order,
    bool IsArchived
);
