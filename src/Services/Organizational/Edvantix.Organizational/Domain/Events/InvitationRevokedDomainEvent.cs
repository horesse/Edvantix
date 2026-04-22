using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

/// <summary>Вызывается при отзыве приглашения отправителем.</summary>
public sealed class InvitationRevokedDomainEvent(Guid organizationId, Guid invitationId)
    : DomainEvent
{
    public Guid OrganizationId { get; } = organizationId;
    public Guid InvitationId { get; } = invitationId;
}
