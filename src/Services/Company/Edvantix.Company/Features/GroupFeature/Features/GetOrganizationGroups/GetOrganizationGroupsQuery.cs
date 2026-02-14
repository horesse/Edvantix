using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate.Specifications;
using Edvantix.Company.Features.GroupFeature.Models;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.GroupFeature.Features.GetOrganizationGroups;

public sealed record GetOrganizationGroupsQuery(long OrganizationId)
    : IRequest<IEnumerable<GroupModel>>;

public sealed class GetOrganizationGroupsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetOrganizationGroupsQuery, IEnumerable<GroupModel>>
{
    public async Task<IEnumerable<GroupModel>> Handle(
        GetOrganizationGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(request.OrganizationId, cancellationToken);

        var spec = new GroupSpecification { OrganizationId = request.OrganizationId };

        using var groupRepo = provider.GetRequiredService<IGroupRepository>();

        var groups = await groupRepo.GetByExpressionAsync(spec, cancellationToken);

        return groups.Select(g => new GroupModel
        {
            Id = g.Id,
            OrganizationId = g.OrganizationId,
            Name = g.Name,
            Description = g.Description,
            MembersCount = g.Members.Count,
        });
    }
}
