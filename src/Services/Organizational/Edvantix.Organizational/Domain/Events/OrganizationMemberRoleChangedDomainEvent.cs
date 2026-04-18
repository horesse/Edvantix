using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

/// <summary>Вызывается при смене роли участника организации.</summary>
public sealed class OrganizationMemberRoleChangedDomainEvent(Guid organizationId, Guid profileId)
    : DomainEvent
{
    public Guid OrganizationId { get; } = organizationId;
    public Guid ProfileId { get; } = profileId;
}
