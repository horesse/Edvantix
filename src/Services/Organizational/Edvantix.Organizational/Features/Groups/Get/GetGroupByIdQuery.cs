using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.Permissions;
using Edvantix.Organizational.Features.OrganizationMembers;
using Edvantix.Organizational.Grpc.Services.Courses;
using Edvantix.Organizational.Grpc.Services.Profiles;

namespace Edvantix.Organizational.Features.Groups.Get;

[RequirePermission(GroupPermissions.View)]
public sealed record GetGroupByIdQuery(Guid Id) : IQuery<GroupDetailDto>;

internal sealed class GetGroupByIdQueryHandler(
    ITenantContext tenantContext,
    IGroupRepository repository,
    IMapper<Group, GroupDetailDto> mapper,
    IProfileService profileService,
    ICurriculumService curriculumService
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

        // Параллельный fan-out: данные для учителя, кабинета и курса запрашиваются одновременно.
        var teacherTask = FetchTeacherDtoAsync(group.TeacherMemberId, cancellationToken);
        var roomLabelTask = FetchRoomLabelAsync(group.RoomId, cancellationToken);
        var coursesTask = curriculumService.GetCoursesByIdsAsync(
            [group.CourseId],
            cancellationToken
        );

        await Task.WhenAll(teacherTask, roomLabelTask, coursesTask);

        // Последовательное применение — каждое with {} работает с актуальным dto.
        var teacher = await teacherTask;
        if (teacher is not null)
            dto = dto with { Teacher = teacher };

        var roomLabel = await roomLabelTask;
        if (roomLabel is not null)
            dto = dto with { RoomLabel = roomLabel };

        var courses = await coursesTask;
        if (courses.TryGetValue(group.CourseId, out var course))
            dto = dto with { CourseCode = course.Code, CourseName = course.Name };

        return dto;
    }

    /// <summary>
    /// Возвращает <see cref="TeacherDto"/> с именем и ролью или <c>null</c>, если профиль не найден.
    /// </summary>
    private async Task<TeacherDto?> FetchTeacherDtoAsync(
        Guid teacherMemberId,
        CancellationToken cancellationToken
    )
    {
        var memberInfo = await repository.GetTeacherMemberInfoAsync(
            [teacherMemberId],
            cancellationToken
        );

        if (!memberInfo.TryGetValue(teacherMemberId, out var member))
            return null;

        var response = await profileService.GetProfilesByIdsAsync(
            [member.ProfileId.ToString()],
            cancellationToken
        );

        var profile = response?.Profiles.FirstOrDefault();

        if (profile is null)
            return null;

        return new TeacherDto(
            MemberId: teacherMemberId,
            FullName: profile.FullName,
            PrimaryRole: member.Role?.Name ?? string.Empty,
            AvatarUrl: profile.HasAvatarUrl ? profile.AvatarUrl : null
        );
    }

    /// <summary>
    /// Возвращает метку кабинета или <c>null</c>, если кабинет не задан или не найден.
    /// </summary>
    private async Task<string?> FetchRoomLabelAsync(
        Guid? roomId,
        CancellationToken cancellationToken
    )
    {
        if (roomId is null)
            return null;

        var rooms = await repository.GetRoomsByIdsAsync([roomId.Value], cancellationToken);

        return rooms.TryGetValue(roomId.Value, out var room) ? room.Label : null;
    }
}
