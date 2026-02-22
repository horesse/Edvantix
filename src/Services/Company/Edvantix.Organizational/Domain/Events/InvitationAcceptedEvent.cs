namespace Edvantix.Organizational.Domain.Events;

/// <summary>
/// Событие принятия приглашения.
/// </summary>
public sealed class InvitationAcceptedEvent(Guid invitationId, Guid organizationId, Guid profileId)
    : DomainEvent
{
    public Guid InvitationId { get; } = invitationId;
    public Guid OrganizationId { get; } = organizationId;
    public Guid ProfileId { get; } = profileId;
}
