using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Specifications;
using Edvantix.Company.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Company.Features.InvitationFeature.Models;
using Edvantix.Company.Grpc.Services;
using MediatR;

namespace Edvantix.Company.Features.InvitationFeature.Features.GetMyInvitations;

/// <summary>
/// Запрос списка приглашений текущего пользователя.
/// </summary>
public sealed record GetMyInvitationsQuery : IRequest<IEnumerable<InvitationModel>>;

/// <summary>
/// Обработчик запроса приглашений текущего пользователя (с именем организации).
/// </summary>
public sealed class GetMyInvitationsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetMyInvitationsQuery, IEnumerable<InvitationModel>>
{
    public async Task<IEnumerable<InvitationModel>> Handle(
        GetMyInvitationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        var spec = new InvitationsByInviteeProfileSpecification(profileId);

        using var invitationRepo = provider.GetRequiredService<IInvitationRepository>();
        var invitations = await invitationRepo.GetByExpressionAsync(spec, cancellationToken);

        if (invitations.Count == 0)
            return [];

        // Получить имена организаций для отображения.
        var organizationIds = invitations
            .Select(x => x.OrganizationId)
            .Distinct()
            .ToList();

        using var orgRepo = provider.GetRequiredService<IOrganizationRepository>();
        var organizations = new Dictionary<long, string>();

        foreach (var orgId in organizationIds)
        {
            var org = await orgRepo.GetByIdAsync(orgId, cancellationToken);
            if (org is not null)
                organizations[orgId] = org.Name;
        }

        return invitations.Select(inv => new InvitationModel
        {
            Id = inv.Id,
            OrganizationId = inv.OrganizationId,
            OrganizationName = organizations.GetValueOrDefault(inv.OrganizationId),
            InvitedByProfileId = inv.InvitedByProfileId,
            InviteeProfileId = inv.InviteeProfileId,
            InviteeEmail = inv.InviteeEmail,
            Role = inv.Role,
            Status = inv.Status,
            Token = inv.Token,
            CreatedAt = inv.CreatedAt,
            ExpiresAt = inv.ExpiresAt,
            RespondedAt = inv.RespondedAt,
        });
    }
}
