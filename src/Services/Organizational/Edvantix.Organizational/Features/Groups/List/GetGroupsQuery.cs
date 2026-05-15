using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.Permissions;
using Edvantix.Organizational.Features.OrganizationMembers;
using Edvantix.Organizational.Grpc.Services.Profiles;
using Edvantix.Organizational.Grpc.Services.Schedules;

namespace Edvantix.Organizational.Features.Groups.List;

[RequirePermission(GroupPermissions.View)]
public sealed record GetGroupsQuery(
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Текстовый поиск по названию")] string? Search = null,
    [property: Description("Фильтр по идентификаторам уровней")] Guid[]? LevelIds = null,
    [property: Description("Фильтр по статусам")] GroupStatus[]? Statuses = null,
    [property: Description("Фильтр по форматам")] GroupFormat[]? Formats = null
) : IQuery<PagedResult<GroupListItemDto>>;

internal sealed class GetGroupsQueryHandler(
    ITenantContext tenantContext,
    IGroupRepository repository,
    IMapper<Group, GroupListItemDto> mapper,
    IProfileService profileService,
    IScheduleService scheduleService
) : IQueryHandler<GetGroupsQuery, PagedResult<GroupListItemDto>>
{
    public async ValueTask<PagedResult<GroupListItemDto>> Handle(
        GetGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        var pageIndex = Math.Max(request.PageIndex, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var offset = (pageIndex - 1) * pageSize;
        var organizationId = tenantContext.OrganizationId;

        var levelIds = request.LevelIds?.Length > 0 ? request.LevelIds : null;
        var statuses = request.Statuses?.Length > 0 ? request.Statuses : null;
        var formats = request.Formats?.Length > 0 ? request.Formats : null;

        var listSpec = new GroupListSpecification(
            organizationId,
            offset,
            pageSize,
            request.Search,
            levelIds,
            statuses,
            formats
        );

        var countSpec = new GroupListSpecification(
            organizationId,
            request.Search,
            levelIds,
            statuses,
            formats
        );

        var groups = await repository.ListAsync(listSpec, cancellationToken);
        var totalCount = await repository.CountAsync(countSpec, cancellationToken);

        var items = groups.Select(mapper.Map).ToList();

        // Параллельное обогащение: учителя, кабинеты и расписания запрашиваются одновременно.
        await Task.WhenAll(
            EnrichWithTeachersAsync(items, groups.ToList(), cancellationToken),
            EnrichWithRoomLabelsAsync(items, groups.ToList(), cancellationToken),
            EnrichWithScheduleSummariesAsync(items, groups.ToList(), cancellationToken)
        );

        return new PagedResult<GroupListItemDto>(items, pageIndex, pageSize, totalCount);
    }

    private async Task EnrichWithTeachersAsync(
        List<GroupListItemDto> items,
        List<Group> groups,
        CancellationToken cancellationToken
    )
    {
        if (items.Count == 0)
            return;

        var teacherMemberIds = groups.Select(g => g.TeacherMemberId).Distinct().ToList();

        var memberInfo = await repository.GetTeacherMemberInfoAsync(
            teacherMemberIds,
            cancellationToken
        );

        if (memberInfo.Count == 0)
            return;

        var profileIds = memberInfo.Values.Select(m => m.ProfileId.ToString()).Distinct().ToArray();
        var response = await profileService.GetProfilesByIdsAsync(profileIds, cancellationToken);

        if (response is null)
            return;

        var profiles = response.Profiles.ToDictionary(p => p.Id);

        for (var i = 0; i < items.Count; i++)
        {
            var teacherMemberId = groups[i].TeacherMemberId;

            if (
                !memberInfo.TryGetValue(teacherMemberId, out var member)
                || !profiles.TryGetValue(member.ProfileId.ToString(), out var profile)
            )
                continue;

            items[i] = items[i] with
            {
                Teacher = new TeacherDto(
                    MemberId: teacherMemberId,
                    FullName: profile.FullName,
                    PrimaryRole: member.Role?.Name ?? string.Empty,
                    AvatarUrl: profile.HasAvatarUrl ? profile.AvatarUrl : null
                ),
            };
        }
    }

    private async Task EnrichWithRoomLabelsAsync(
        List<GroupListItemDto> items,
        List<Group> groups,
        CancellationToken cancellationToken
    )
    {
        var roomIds = groups
            .Where(g => g.RoomId.HasValue)
            .Select(g => g.RoomId!.Value)
            .Distinct()
            .ToList();

        if (roomIds.Count == 0)
            return;

        var rooms = await repository.GetRoomsByIdsAsync(roomIds, cancellationToken);

        for (var i = 0; i < items.Count; i++)
        {
            if (groups[i].RoomId is { } roomId && rooms.TryGetValue(roomId, out var room))
            {
                items[i] = items[i] with { RoomLabel = room.Label };
            }
        }
    }

    private async Task EnrichWithScheduleSummariesAsync(
        List<GroupListItemDto> items,
        List<Group> groups,
        CancellationToken cancellationToken
    )
    {
        if (items.Count == 0)
            return;

        var groupIds = groups.Select(g => g.Id).Distinct().ToList();

        var summaries = await scheduleService.GetScheduleSummariesAsync(
            groupIds,
            cancellationToken
        );

        if (summaries.Count == 0)
            return;

        for (var i = 0; i < items.Count; i++)
        {
            if (summaries.TryGetValue(groups[i].Id, out var summary))
            {
                items[i] = items[i] with { ScheduleSummary = summary };
            }
        }
    }
}
