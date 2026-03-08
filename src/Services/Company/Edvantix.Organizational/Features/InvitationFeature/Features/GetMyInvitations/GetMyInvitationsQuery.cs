using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Features.InvitationFeature.Models;

namespace Edvantix.Organizational.Features.InvitationFeature.Features.GetMyInvitations;

/// <summary>
/// Запрос списка приглашений текущего пользователя.
/// </summary>
public sealed record GetMyInvitationsQuery : IQuery<IEnumerable<InvitationModel>>;

/// <summary>
/// Обработчик запроса приглашений текущего пользователя (с именем организации).
/// </summary>
public sealed class GetMyInvitationsQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetMyInvitationsQuery, IEnumerable<InvitationModel>>
{
    public async ValueTask<IEnumerable<InvitationModel>> Handle(
        GetMyInvitationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileId = provider.GetProfileIdOrError();

        // Получаем все ожидающие приглашения для профиля (без фильтра по организации)
        var spec = new InvitationSpecification(profileId: profileId, organizationId: null);
        var invitationRepo = provider.GetRequiredService<IInvitationRepository>();
        var invitations = await invitationRepo.ListAsync(spec, cancellationToken);

        if (invitations.Count == 0)
            return [];

        // Получить имена организаций для отображения
        var organizationIds = invitations.Select(x => x.OrganizationId).Distinct().ToList();
        var orgRepo = provider.GetRequiredService<IOrganizationRepository>();
        var organizations = new Dictionary<Guid, string>();

        foreach (var orgId in organizationIds)
        {
            var org = await orgRepo.FindByIdAsync(orgId, cancellationToken);
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
