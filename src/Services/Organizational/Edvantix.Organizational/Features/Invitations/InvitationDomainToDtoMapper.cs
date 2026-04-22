using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

namespace Edvantix.Organizational.Features.Invitations;

internal sealed class InvitationDomainToDtoMapper : Mapper<Invitation, InvitationDto>
{
    public override InvitationDto Map(Invitation source) =>
        new(
            source.Id,
            source.OrganizationId,
            source.InviterProfileId,
            source.RoleId,
            source.Type,
            source.Status,
            source.Email,
            source.InviteeLogin,
            source.ExpiresAt,
            source.CreatedAt,
            source.AcceptedAt
        );
}
