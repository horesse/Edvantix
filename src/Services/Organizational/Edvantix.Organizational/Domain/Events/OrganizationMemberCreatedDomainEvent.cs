using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

/// <summary>Вызывается при добавлении нового участника в организацию.</summary>
public sealed class OrganizationMemberCreatedDomainEvent(Guid organizationId, Guid profileId)
    : DomainEvent
{
    public Guid OrganizationId { get; } = organizationId;
    public Guid ProfileId { get; } = profileId;
}
