using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

public sealed class OrganizationCreatedDomainEvent(Guid organizationId, Guid ownerProfileId)
    : DomainEvent
{
    public Guid OrganizationId { get; } = organizationId;
    public Guid OwnerProfileId { get; } = ownerProfileId;
}
