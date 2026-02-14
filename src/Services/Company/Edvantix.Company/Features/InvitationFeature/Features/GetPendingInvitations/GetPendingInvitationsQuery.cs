using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Specifications;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Features.InvitationFeature.Models;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.InvitationFeature.Features.GetPendingInvitations;

/// <summary>
/// Запрос списка ожидающих приглашений организации.
/// </summary>
public sealed record GetPendingInvitationsQuery(long OrganizationId)
    : IRequest<IEnumerable<InvitationModel>>;

/// <summary>
/// Обработчик запроса ожидающих приглашений. Доступно Owner/Manager.
/// </summary>
public sealed class GetPendingInvitationsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetPendingInvitationsQuery, IEnumerable<InvitationModel>>
{
    public async Task<IEnumerable<InvitationModel>> Handle(
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

        var spec = new PendingInvitationsByOrganizationSpecification(request.OrganizationId);

        using var invitationRepo = provider.GetRequiredService<IInvitationRepository>();
        var invitations = await invitationRepo.GetByExpressionAsync(spec, cancellationToken);

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
