namespace Edvantix.Organizational.Domain.Events;

/// <summary>
/// Событие отклонения приглашения.
/// </summary>
public sealed class InvitationDeclinedEvent(Guid invitationId, Guid organizationId) : DomainEvent
{
    public Guid InvitationId { get; } = invitationId;
    public Guid OrganizationId { get; } = organizationId;
}
