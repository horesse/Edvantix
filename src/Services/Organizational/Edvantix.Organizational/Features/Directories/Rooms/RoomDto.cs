using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.Features.Directories.Rooms;

/// <summary>Полное DTO кабинета (используется в GetById и после Create/Update).</summary>
/// <param name="Id">Идентификатор записи.</param>
/// <param name="Name">Название кабинета.</param>
/// <param name="Capacity">Вместимость (1–1000).</param>
/// <param name="Floor">Номер/название этажа или <c>null</c>.</param>
/// <param name="RoomType">Тип помещения.</param>
/// <param name="IsArchived">Признак архивации.</param>
/// <param name="Order">Порядок сортировки.</param>
/// <param name="OrganizationId">Идентификатор организации.</param>
/// <param name="CreatedAt">Дата создания.</param>
/// <param name="LastModifiedAt">Дата последнего изменения.</param>
/// <param name="CreatedBy">Кто создал.</param>
/// <param name="LastModifiedBy">Кто изменил последним.</param>
public sealed record RoomDto(
    Guid Id,
    string Name,
    int Capacity,
    string? Floor,
    RoomType RoomType,
    bool IsArchived,
    int Order,
    Guid OrganizationId,
    DateTime CreatedAt,
    DateTime? LastModifiedAt,
    Guid? CreatedBy,
    Guid? LastModifiedBy
);

/// <summary>Краткое DTO кабинета для постраничного списка.</summary>
/// <param name="Id">Идентификатор записи (ключ строки в UI).</param>
/// <param name="Name">Название кабинета.</param>
/// <param name="Capacity">Вместимость.</param>
/// <param name="Floor">Номер/название этажа или <c>null</c>.</param>
/// <param name="RoomType">Тип помещения.</param>
/// <param name="IsArchived">Признак архивации.</param>
/// <param name="Order">Порядок сортировки.</param>
public sealed record RoomListItemDto(
    Guid Id,
    string Name,
    int Capacity,
    string? Floor,
    RoomType RoomType,
    bool IsArchived,
    int Order
);
