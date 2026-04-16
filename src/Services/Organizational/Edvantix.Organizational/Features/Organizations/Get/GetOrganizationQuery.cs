using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Features.Organizations;

namespace Edvantix.Organizational.Features.Organizations.Get;

public sealed record GetOrganizationQuery(Guid Id) : IQuery<OrganizationDetailDto>;

internal sealed class GetOrganizationQueryHandler(
    IOrganizationRepository repository,
    IMapper<Organization, OrganizationDetailDto> mapper
) : IQueryHandler<GetOrganizationQuery, OrganizationDetailDto>
{
    public async ValueTask<OrganizationDetailDto> Handle(
        GetOrganizationQuery query,
        CancellationToken cancellationToken
    )
    {
        var organization = await repository.GetByIdAsync(query.Id, cancellationToken);
        Guard.Against.NotFound(organization, query.Id);

        return mapper.Map(organization);
    }
}
