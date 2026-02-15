using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Events;

/// <summary>
/// Событие создания приглашения (для будущей email-нотификации).
/// </summary>
public sealed class InvitationCreatedEvent(
    Guid invitationId,
    long organizationId,
    string? inviteeEmail,
    long? inviteeProfileId
) : DomainEvent
{
    public Guid InvitationId { get; } = invitationId;
    public long OrganizationId { get; } = organizationId;
    public string? InviteeEmail { get; } = inviteeEmail;
    public long? InviteeProfileId { get; } = inviteeProfileId;
}
