namespace Edvantix.Organizational.Features.Directories.PaymentMethods;

/// <summary>Полное DTO способа оплаты (используется в GetById и после Create/Update).</summary>
/// <param name="Id">Идентификатор записи.</param>
/// <param name="Name">Название способа оплаты.</param>
/// <param name="Code">Машинный код.</param>
/// <param name="IsCashless">Признак безналичного расчёта.</param>
/// <param name="RequiresContract">Признак необходимости договора.</param>
/// <param name="IsArchived">Признак архивации.</param>
/// <param name="Order">Порядок сортировки.</param>
/// <param name="OrganizationId">Идентификатор организации.</param>
/// <param name="CreatedAt">Дата создания.</param>
/// <param name="LastModifiedAt">Дата последнего изменения.</param>
/// <param name="CreatedBy">Кто создал.</param>
/// <param name="LastModifiedBy">Кто изменил последним.</param>
public sealed record PaymentMethodDto(
    Guid Id,
    string Name,
    string Code,
    bool IsCashless,
    bool RequiresContract,
    bool IsArchived,
    int Order,
    Guid OrganizationId,
    DateTime CreatedAt,
    DateTime? LastModifiedAt,
    Guid? CreatedBy,
    Guid? LastModifiedBy
);

/// <summary>Краткое DTO способа оплаты для постраничного списка.</summary>
/// <param name="Id">Идентификатор записи (ключ строки в UI).</param>
/// <param name="Name">Название способа оплаты.</param>
/// <param name="Code">Машинный код.</param>
/// <param name="IsCashless">Признак безналичного расчёта.</param>
/// <param name="RequiresContract">Признак необходимости договора.</param>
/// <param name="IsArchived">Признак архивации.</param>
/// <param name="Order">Порядок сортировки.</param>
public sealed record PaymentMethodListItemDto(
    Guid Id,
    string Name,
    string Code,
    bool IsCashless,
    bool RequiresContract,
    bool IsArchived,
    int Order
);
