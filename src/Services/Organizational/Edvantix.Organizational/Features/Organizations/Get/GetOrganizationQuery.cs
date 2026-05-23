using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using ZiggyCreatures.Caching.Fusion;

namespace Edvantix.Organizational.Features.Organizations.Get;

public sealed record GetOrganizationQuery(Guid Id) : IQuery<OrganizationDetailDto>;

internal sealed class GetOrganizationQueryHandler(
    IFusionCache cache,
    IOrganizationRepository repository,
    IMapper<Organization, OrganizationDetailDto> mapper
) : IQueryHandler<GetOrganizationQuery, OrganizationDetailDto>
{
    public async ValueTask<OrganizationDetailDto> Handle(
        GetOrganizationQuery query,
        CancellationToken cancellationToken
    )
    {
        var tag = nameof(Organization).ToLowerInvariant();

        var organization = await cache.GetOrSetAsync(
            $"{tag}:{query.Id}",
            async ctx =>
            {
                var organization = await repository.GetByIdAsync(query.Id, ctx);
                Guard.Against.NotFound(organization, query.Id);

                return organization;
            },
            tags: [tag],
            token: cancellationToken
        );

        return mapper.Map(organization);
    }
}
