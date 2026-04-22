using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

/// <summary>Вызывается при отклонении приглашения адресатом.</summary>
public sealed class InvitationDeclinedDomainEvent(Guid organizationId, Guid invitationId)
    : DomainEvent
{
    public Guid OrganizationId { get; } = organizationId;
    public Guid InvitationId { get; } = invitationId;
}
