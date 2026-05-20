using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Groups.Domain.Permissions;
using Edvantix.Groups.Grpc.Services.Profiles;

namespace Edvantix.Groups.Features.Groups.Members.List;

[RequirePermission(GroupPermissions.Members)]
public sealed record GetGroupMembersQuery(
    Guid GroupId,
    [property: Description("Включить выбывших участников")]
    [property: DefaultValue(false)]
        bool IncludeExited = false,
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize
) : IQuery<PagedResult<GroupMemberDto>>;

internal sealed class GetGroupMembersQueryHandler(
    ITenantContext tenantContext,
    IGroupRepository repository,
    IMapper<GroupMember, GroupMemberDto> mapper,
    IProfileService profileService
) : IQueryHandler<GetGroupMembersQuery, PagedResult<GroupMemberDto>>
{
    public async ValueTask<PagedResult<GroupMemberDto>> Handle(
        GetGroupMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var group = await repository.GetByIdAsync(request.GroupId, cancellationToken);
        Guard.Against.NotFound(group, request.GroupId);

        if (group.OrganizationId != tenantContext.OrganizationId)
            throw new ForbiddenException("Группа не принадлежит текущей организации.");

        var pageIndex = Math.Max(request.PageIndex, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var filtered = (
            request.IncludeExited
                ? group.Members.AsEnumerable()
                : group.Members.Where(m => m.ExitedAt is null)
        ).ToList();

        var totalCount = filtered.Count;
        var page = filtered.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        var items = page.Select(mapper.Map).ToList();

        await EnrichWithProfilesAsync(items, page, cancellationToken);

        return new PagedResult<GroupMemberDto>(items, pageIndex, pageSize, totalCount);
    }

    private async Task EnrichWithProfilesAsync(
        List<GroupMemberDto> items,
        List<GroupMember> members,
        CancellationToken cancellationToken
    )
    {
        if (items.Count == 0)
            return;

        var profileIds = members.Select(m => m.ProfileId.ToString()).Distinct().ToArray();
        var response = await profileService.GetProfilesByIdsAsync(profileIds, cancellationToken);

        if (response is null)
            return;

        var profiles = response.Profiles.ToDictionary(p => p.Id);

        for (var i = 0; i < items.Count; i++)
        {
            var profileId = members[i].ProfileId.ToString();

            if (!profiles.TryGetValue(profileId, out var profile))
                continue;

            items[i] = items[i] with
            {
                FullName = profile.FullName,
                AvatarUrl = profile.HasAvatarUrl ? profile.AvatarUrl : null,
            };
        }
    }
}
