using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

/// <summary>Вызывается при изменении набора разрешений роли участника организации.</summary>
public sealed class OrganizationRolePermissionsChangedDomainEvent(Guid organizationId) : DomainEvent
{
    public Guid OrganizationId { get; } = organizationId;
}
