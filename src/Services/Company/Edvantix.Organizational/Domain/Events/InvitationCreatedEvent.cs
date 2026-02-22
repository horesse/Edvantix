namespace Edvantix.Organizational.Domain.Events;

/// <summary>
/// Событие создания приглашения (для будущей email-нотификации).
/// </summary>
public sealed class InvitationCreatedEvent(
    Guid invitationId,
    Guid organizationId,
    string? inviteeEmail,
    Guid? inviteeProfileId
) : DomainEvent
{
    public Guid InvitationId { get; } = invitationId;
    public Guid OrganizationId { get; } = organizationId;
    public string? InviteeEmail { get; } = inviteeEmail;
    public Guid? InviteeProfileId { get; } = inviteeProfileId;
}
