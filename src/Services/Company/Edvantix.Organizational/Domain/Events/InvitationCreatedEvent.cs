namespace Edvantix.Organizational.Domain.Events;

/// <summary>
/// Событие создания приглашения (для будущей email-нотификации).
/// </summary>
public sealed class InvitationCreatedEvent(
    Guid invitationId,
    ulong organizationId,
    string? inviteeEmail,
    ulong? inviteeProfileId
) : DomainEvent
{
    public Guid InvitationId { get; } = invitationId;
    public ulong OrganizationId { get; } = organizationId;
    public string? InviteeEmail { get; } = inviteeEmail;
    public ulong? InviteeProfileId { get; } = inviteeProfileId;
}
