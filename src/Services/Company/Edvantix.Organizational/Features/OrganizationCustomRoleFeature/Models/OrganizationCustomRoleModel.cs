using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Models;

/// <summary>
/// Модель кастомной роли организации для передачи данных клиенту.
/// </summary>
public sealed record OrganizationCustomRoleModel
{
    /// <summary>Уникальный идентификатор роли.</summary>
    public Guid Id { get; init; }

    /// <summary>Идентификатор организации.</summary>
    public Guid OrganizationId { get; init; }

    /// <summary>Уникальный код роли в рамках организации.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>Описание роли и её полномочий.</summary>
    public string? Description { get; init; }

    /// <summary>Базовая роль, определяющая уровень доступа.</summary>
    public OrganizationBaseRole BaseRole { get; init; }
}
