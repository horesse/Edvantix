namespace Edvantix.Organizational.Features.Directories.StudentTags;

/// <summary>Полное DTO тега студента (используется в GetById и после Create/Update).</summary>
/// <param name="Id">Идентификатор записи.</param>
/// <param name="Name">Название тега.</param>
/// <param name="Color">Цвет тега в формате HEX <c>#RRGGBB</c>.</param>
/// <param name="IsArchived">Признак архивации.</param>
/// <param name="Order">Порядок сортировки.</param>
/// <param name="OrganizationId">Идентификатор организации.</param>
/// <param name="CreatedAt">Дата создания.</param>
/// <param name="LastModifiedAt">Дата последнего изменения.</param>
/// <param name="CreatedBy">Кто создал.</param>
/// <param name="LastModifiedBy">Кто изменил последним.</param>
public sealed record StudentTagDto(
    Guid Id,
    string Name,
    string Color,
    bool IsArchived,
    int Order,
    Guid OrganizationId,
    DateTime CreatedAt,
    DateTime? LastModifiedAt,
    Guid? CreatedBy,
    Guid? LastModifiedBy
);

/// <summary>Краткое DTO тега студента для постраничного списка.</summary>
/// <param name="Id">Идентификатор записи (ключ строки в UI).</param>
/// <param name="Name">Название тега.</param>
/// <param name="Color">Цвет тега в формате HEX <c>#RRGGBB</c>.</param>
/// <param name="IsArchived">Признак архивации.</param>
/// <param name="Order">Порядок сортировки.</param>
public sealed record StudentTagListItemDto(
    Guid Id,
    string Name,
    string Color,
    bool IsArchived,
    int Order
);
