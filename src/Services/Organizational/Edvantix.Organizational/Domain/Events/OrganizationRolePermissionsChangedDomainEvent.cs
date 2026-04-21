using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

/// <summary>Вызывается при изменении набора разрешений роли участника организации.</summary>
public sealed class OrganizationRolePermissionsChangedDomainEvent(Guid organizationId, Guid roleId)
    : DomainEvent
{
    public Guid OrganizationId { get; } = organizationId;

    /// <summary>Идентификатор роли, чьи разрешения изменились — используется для точечной инвалидации кеша.</summary>
    public Guid RoleId { get; } = roleId;
}
