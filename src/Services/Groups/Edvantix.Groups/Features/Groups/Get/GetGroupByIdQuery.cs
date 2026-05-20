using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Groups.Grpc.Services.Courses;
using Edvantix.Groups.Grpc.Services.Schedules;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Groups.Get;

[RequirePermission(GroupPermissions.View)]
public sealed record GetGroupByIdQuery(Guid Id) : IQuery<GroupDetailDto>;

internal sealed class GetGroupByIdQueryHandler(
    ITenantContext tenantContext,
    IGroupRepository repository,
    IMapper<Group, GroupDetailDto> mapper,
    ICurriculumService curriculumService,
    IScheduleService scheduleService
) : IQueryHandler<GetGroupByIdQuery, GroupDetailDto>
{
    public async ValueTask<GroupDetailDto> Handle(
        GetGroupByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var group = await repository.GetByIdAsync(query.Id, cancellationToken);
        Guard.Against.NotFound(group, query.Id);

        if (group.OrganizationId != tenantContext.OrganizationId)
            throw new ForbiddenException("Группа не принадлежит текущей организации.");

        var dto = mapper.Map(group);

        // Параллельный fan-out: данные для курса и расписания запрашиваются одновременно.
        var coursesTask = curriculumService.GetCoursesByIdsAsync(
            [group.CourseId],
            cancellationToken
        );
        var scheduleTask = scheduleService.GetScheduleByGroupIdAsync(group.Id, cancellationToken);
        var upcomingTask = scheduleService.GetUpcomingLessonsAsync(
            group.Id,
            count: 5,
            cancellationToken
        );

        await Task.WhenAll(coursesTask, scheduleTask, upcomingTask);

        var courses = await coursesTask;
        if (courses.TryGetValue(group.CourseId, out var course))
            dto = dto with { CourseCode = course.Code, CourseName = course.Name };

        dto = dto with { Schedule = await scheduleTask, UpcomingLessons = await upcomingTask };

        return dto;
    }
}
