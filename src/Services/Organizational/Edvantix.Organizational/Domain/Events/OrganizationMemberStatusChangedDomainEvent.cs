using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

/// <summary>Вызывается при деактивации или удалении участника организации.</summary>
public sealed class OrganizationMemberStatusChangedDomainEvent(Guid organizationId, Guid profileId)
    : DomainEvent
{
    public Guid OrganizationId { get; } = organizationId;
    public Guid ProfileId { get; } = profileId;
}
