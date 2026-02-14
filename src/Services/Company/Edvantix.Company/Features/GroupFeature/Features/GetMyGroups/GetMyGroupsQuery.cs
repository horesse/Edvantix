using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate.Specifications;
using Edvantix.Company.Features.GroupFeature.Models;
using Edvantix.Company.Grpc.Services;
using MediatR;

namespace Edvantix.Company.Features.GroupFeature.Features.GetMyGroups;

public sealed record GetMyGroupsQuery : IRequest<IEnumerable<GroupSummaryModel>>;

public sealed class GetMyGroupsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetMyGroupsQuery, IEnumerable<GroupSummaryModel>>
{
    public async Task<IEnumerable<GroupSummaryModel>> Handle(
        GetMyGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        var spec = new GroupsByProfileSpecification(profileId);

        using var groupMemberRepo = provider.GetRequiredService<IGroupMemberRepository>();

        var memberships = await groupMemberRepo.GetByExpressionAsync(spec, cancellationToken);

        if (memberships.Count == 0)
            return [];

        using var groupRepo = provider.GetRequiredService<IGroupRepository>();

        var result = new List<GroupSummaryModel>();

        foreach (var membership in memberships)
        {
            var group = await groupRepo.GetByIdAsync(membership.GroupId, cancellationToken);
            if (group is null)
                continue;

            result.Add(
                new GroupSummaryModel(
                    group.Id,
                    group.OrganizationId,
                    group.Name,
                    group.Description,
                    membership.Role.ToString()
                )
            );
        }

        return result;
    }
}
