using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Events;

/// <summary>
/// Событие принятия приглашения.
/// </summary>
public sealed class InvitationAcceptedEvent(Guid invitationId, long organizationId, long profileId)
    : DomainEvent
{
    public Guid InvitationId { get; } = invitationId;
    public long OrganizationId { get; } = organizationId;
    public long ProfileId { get; } = profileId;
}
