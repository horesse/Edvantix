using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;

namespace Edvantix.Organizational.Features.Directories.LeadSources;

/// <summary>Полное DTO источника привлечения (используется в GetById и после Create/Update).</summary>
/// <param name="Id">Идентификатор записи.</param>
/// <param name="Name">Название источника.</param>
/// <param name="Channel">Канал привлечения.</param>
/// <param name="UtmTag">UTM-метка для атрибуции или <c>null</c>.</param>
/// <param name="IsArchived">Признак архивации.</param>
/// <param name="Order">Порядок сортировки.</param>
/// <param name="OrganizationId">Идентификатор организации.</param>
/// <param name="CreatedAt">Дата создания.</param>
/// <param name="LastModifiedAt">Дата последнего изменения.</param>
/// <param name="CreatedBy">Кто создал.</param>
/// <param name="LastModifiedBy">Кто изменил последним.</param>
public sealed record LeadSourceDto(
    Guid Id,
    string Name,
    LeadChannel Channel,
    string? UtmTag,
    bool IsArchived,
    int Order,
    Guid OrganizationId,
    DateTime CreatedAt,
    DateTime? LastModifiedAt,
    Guid? CreatedBy,
    Guid? LastModifiedBy
);

/// <summary>Краткое DTO источника привлечения для постраничного списка.</summary>
/// <param name="Id">Идентификатор записи (ключ строки в UI).</param>
/// <param name="Name">Название источника.</param>
/// <param name="Channel">Канал привлечения.</param>
/// <param name="UtmTag">UTM-метка для атрибуции или <c>null</c>.</param>
/// <param name="IsArchived">Признак архивации.</param>
/// <param name="Order">Порядок сортировки.</param>
public sealed record LeadSourceListItemDto(
    Guid Id,
    string Name,
    LeadChannel Channel,
    string? UtmTag,
    bool IsArchived,
    int Order
);
