using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses;

/// <summary>Полное DTO статуса студента (используется в GetById и после Create/Update).</summary>
/// <param name="Id">Идентификатор записи.</param>
/// <param name="Name">Отображаемое имя.</param>
/// <param name="Code">Машинный код.</param>
/// <param name="Tone">Визуальный тон UI.</param>
/// <param name="IsSystem">Системная запись (нельзя архивировать).</param>
/// <param name="IsArchived">Признак архивации.</param>
/// <param name="Order">Порядок сортировки.</param>
/// <param name="OrganizationId">Идентификатор организации.</param>
/// <param name="CreatedAt">Дата создания.</param>
/// <param name="LastModifiedAt">Дата последнего изменения.</param>
/// <param name="CreatedBy">Кто создал.</param>
/// <param name="LastModifiedBy">Кто изменил последним.</param>
public sealed record StudentStatusDto(
    Guid Id,
    string Name,
    string Code,
    StudentStatusTone Tone,
    bool IsSystem,
    bool IsArchived,
    int Order,
    Guid OrganizationId,
    DateTime CreatedAt,
    DateTime? LastModifiedAt,
    Guid? CreatedBy,
    Guid? LastModifiedBy
);
