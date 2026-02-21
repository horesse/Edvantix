using Edvantix.Organizational.Features.GroupFeature.Models;
using Edvantix.Organizational.Grpc.Services;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Organizational.Features.GroupFeature.Features.GetMyGroups;

public sealed record GetMyGroupsQuery(
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description(
        "Количество элементов, которые должны быть отображены на одной странице результатов."
    )]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Свойство для упорядочивания результатов")] string? OrderBy = null,
    [property: Description("При выборе порядка сортировки результат будет в порядке убывания.")]
    [property: DefaultValue(false)]
        bool IsDescending = false
) : IRequest<PagedResult<GroupSummaryModel>>;

public sealed class GetMyGroupsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetMyGroupsQuery, PagedResult<GroupSummaryModel>>
{
    public async ValueTask<PagedResult<GroupSummaryModel>> Handle(
        GetMyGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        var spec = new GroupMemberSpecification(profileId);

        var groupMemberRepo = provider.GetRequiredService<IGroupMemberRepository>();

        var count = await groupMemberRepo.CountAsync(spec, cancellationToken);

        spec.Skip = (request.PageIndex - 1) * request.PageSize;
        spec.Take = request.PageSize;

        var memberships = await groupMemberRepo.ListAsync(spec, cancellationToken);

        if (memberships.Count == 0)
            return new PagedResult<GroupSummaryModel>(
                [],
                request.PageIndex,
                request.PageSize,
                count
            );

        var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var result = new List<GroupSummaryModel>(memberships.Count);

        foreach (var membership in memberships)
        {
            var group = await groupRepo.FindByIdAsync(membership.GroupId, cancellationToken);
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

        return new PagedResult<GroupSummaryModel>(
            result,
            request.PageIndex,
            request.PageSize,
            count
        );
    }
}
