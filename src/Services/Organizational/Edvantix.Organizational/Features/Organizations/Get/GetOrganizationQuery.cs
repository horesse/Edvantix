using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

namespace Edvantix.Organizational.Features.Organizations.Get;

public sealed record GetOrganizationQuery(Guid Id) : IQuery<OrganizationDetailDto>;

internal sealed class GetOrganizationQueryHandler(
    IHybridCache cache,
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

        var organization = await cache.GetOrCreateAsync(
            $"{tag}:{query.Id}",
            async ctx =>
            {
                var organization = await repository.GetByIdAsync(query.Id, ctx);
                Guard.Against.NotFound(organization, query.Id);

                return organization;
            },
            [tag],
            cancellationToken
        );

        return mapper.Map(organization);
    }
}
