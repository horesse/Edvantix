using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.Events;

/// <summary>Вызывается при принятии приглашения. Инициирует создание участника организации.</summary>
public sealed class InvitationAcceptedDomainEvent(
    Guid organizationId,
    Guid acceptedByProfileId,
    Guid roleId
) : DomainEvent
{
    public Guid OrganizationId { get; } = organizationId;
    public Guid AcceptedByProfileId { get; } = acceptedByProfileId;
    public Guid RoleId { get; } = roleId;
}
