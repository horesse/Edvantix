using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Events;

/// <summary>
/// Событие отклонения приглашения.
/// </summary>
public sealed class InvitationDeclinedEvent(Guid invitationId, long organizationId) : DomainEvent
{
    public Guid InvitationId { get; } = invitationId;
    public long OrganizationId { get; } = organizationId;
}
