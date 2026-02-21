namespace Edvantix.Organizational.Domain.Events;

/// <summary>
/// Событие отклонения приглашения.
/// </summary>
public sealed class InvitationDeclinedEvent(Guid invitationId, ulong organizationId) : DomainEvent
{
    public Guid InvitationId { get; } = invitationId;
    public ulong OrganizationId { get; } = organizationId;
}
