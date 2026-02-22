using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Features.InvitationFeature.Models;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.InvitationFeature.Features.GetPendingInvitations;

/// <summary>
/// Запрос списка ожидающих приглашений организации.
/// </summary>
public sealed record GetPendingInvitationsQuery(Guid OrganizationId)
    : IRequest<IEnumerable<InvitationModel>>;

/// <summary>
/// Обработчик запроса ожидающих приглашений. Доступно Owner/Manager.
/// </summary>
public sealed class GetPendingInvitationsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetPendingInvitationsQuery, IEnumerable<InvitationModel>>
{
    public async ValueTask<IEnumerable<InvitationModel>> Handle(
        GetPendingInvitationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireOrgRoleAsync(
            request.OrganizationId,
            cancellationToken,
            OrganizationRole.Owner,
            OrganizationRole.Manager
        );

        // Pending-приглашения по organizationId (profileId не задан — все приглашения орга)
        var spec = new InvitationSpecification(organizationId: request.OrganizationId);
        var invitationRepo = provider.GetRequiredService<IInvitationRepository>();
        var invitations = await invitationRepo.ListAsync(spec, cancellationToken);

        return invitations.Select(MapToModel);
    }

    private static InvitationModel MapToModel(Invitation invitation) =>
        new()
        {
            Id = invitation.Id,
            OrganizationId = invitation.OrganizationId,
            InvitedByProfileId = invitation.InvitedByProfileId,
            InviteeProfileId = invitation.InviteeProfileId,
            InviteeEmail = invitation.InviteeEmail,
            Role = invitation.Role,
            Status = invitation.Status,
            Token = invitation.Token,
            CreatedAt = invitation.CreatedAt,
            ExpiresAt = invitation.ExpiresAt,
            RespondedAt = invitation.RespondedAt,
        };
}
