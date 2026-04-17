using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

public sealed class OrganizationDeletedDomainEvent(Guid organizationId) : DomainEvent
{
    public Guid OrganizationId { get; } = organizationId;
}
