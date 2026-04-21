using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Features.Organizations.ListByProfile;

public sealed record GetMyOrganizationsQuery : IQuery<IReadOnlyList<OrganizationWithRoleDto>>;

internal sealed class GetMyOrganizationsQueryHandler(
    ClaimsPrincipal claims,
    IOrganizationMemberRepository memberRepository,
    IOrganizationRepository organizationRepository
) : IQueryHandler<GetMyOrganizationsQuery, IReadOnlyList<OrganizationWithRoleDto>>
{
    public async ValueTask<IReadOnlyList<OrganizationWithRoleDto>> Handle(
        GetMyOrganizationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileId = claims.GetProfileIdOrError();

        var spec = new OrganizationsByProfileSpecification(profileId);
        var members = await memberRepository.ListAsync(spec, cancellationToken);

        if (members.Count == 0)
        {
            return [];
        }

        var organizationIds = members.Select(m => m.OrganizationId).Distinct().ToList();
        var orgSpec = new OrganizationsByIdsSpecification(organizationIds);
        var organizations = await organizationRepository.ListAsync(orgSpec, cancellationToken);

        var orgLookup = organizations.ToDictionary(o => o.Id);

        return
        [
            .. members
                .Where(m => orgLookup.ContainsKey(m.OrganizationId) && m.Role is not null)
                .Select(m => new OrganizationWithRoleDto(
                    orgLookup[m.OrganizationId].Id,
                    orgLookup[m.OrganizationId].FullLegalName,
                    orgLookup[m.OrganizationId].ShortName,
                    orgLookup[m.OrganizationId].OrganizationType,
                    orgLookup[m.OrganizationId].Status,
                    orgLookup[m.OrganizationId].IsLegalEntity,
                    m.Role!.Code,
                    m.Role.Description
                )),
        ];
    }
}
