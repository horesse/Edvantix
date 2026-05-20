using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Groups.Grpc.Services.Courses;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Groups.List;

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
    ICurriculumService curriculumService
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

        await EnrichWithCoursesAsync(items, groups.ToList(), cancellationToken);

        return new PagedResult<GroupListItemDto>(items, pageIndex, pageSize, totalCount);
    }

    private async Task EnrichWithCoursesAsync(
        List<GroupListItemDto> items,
        List<Group> groups,
        CancellationToken cancellationToken
    )
    {
        if (items.Count == 0)
            return;

        var courseIds = groups.Select(g => g.CourseId).Distinct();
        var courses = await curriculumService.GetCoursesByIdsAsync(courseIds, cancellationToken);

        if (courses.Count == 0)
            return;

        for (var i = 0; i < items.Count; i++)
        {
            if (courses.TryGetValue(groups[i].CourseId, out var course))
            {
                items[i] = items[i] with { CourseCode = course.Code, CourseName = course.Name };
            }
        }
    }
}
