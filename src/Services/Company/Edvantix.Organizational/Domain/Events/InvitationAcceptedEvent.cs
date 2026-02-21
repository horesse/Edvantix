namespace Edvantix.Organizational.Domain.Events;

/// <summary>
/// Событие принятия приглашения.
/// </summary>
public sealed class InvitationAcceptedEvent(
    Guid invitationId,
    ulong organizationId,
    ulong profileId
) : DomainEvent
{
    public Guid InvitationId { get; } = invitationId;
    public ulong OrganizationId { get; } = organizationId;
    public ulong ProfileId { get; } = profileId;
}
