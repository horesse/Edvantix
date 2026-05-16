using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Features.OrganizationMembers;

namespace Edvantix.Organizational.Features.Groups;

/// <summary>
/// Маппер Group → GroupListItemDto.
/// <para>
/// Level-поля берутся из navigation property <c>Group.Level</c>, которое всегда подгружается
/// через <c>AutoInclude</c> в <c>GroupConfiguration</c> — отдельного запроса не требуется.
/// Course-поля (Code, Name) инициализируются пустой строкой; handler обогащает их через
/// батч-вызов <c>ICurriculumService.GetCoursesByIdsAsync</c>.
/// </para>
/// </summary>
public sealed class GroupListItemDtoMapper : Mapper<Group, GroupListItemDto>
{
    public override GroupListItemDto Map(Group source) =>
        new(
            source.Id,
            source.Code.Value,
            source.Name,
            LevelId: source.LevelId,
            LevelCode: source.Level.Code.Value,
            LevelName: source.Level.Name,
            LevelTone: source.Level.Tone,
            CourseId: source.CourseId,
            CourseCode: string.Empty,
            CourseName: string.Empty,
            Teacher: new TeacherDto(source.TeacherMemberId, string.Empty, string.Empty, null),
            source.RoomId,
            RoomLabel: null,
            source.Format,
            source.Platform,
            ScheduleSummary: null,
            source.Capacity,
            MemberCount: source.Members.Count(m => m.ExitedAt is null),
            source.Status,
            source.StartDate,
            source.EndDate
        );
}

/// <summary>
/// Маппер Group → GroupDetailDto.
/// <para>
/// Level-поля берутся из navigation property <c>Group.Level</c> (AutoInclude).
/// Course-поля (Code, Name) инициализируются пустой строкой; handler обогащает их через
/// батч-вызов <c>ICurriculumService.GetCoursesByIdsAsync</c>.
/// Schedule и UpcomingLessons — placeholder; заполняется в Task 8.
/// </para>
/// </summary>
public sealed class GroupDetailDtoMapper : Mapper<Group, GroupDetailDto>
{
    public override GroupDetailDto Map(Group source) =>
        new(
            source.Id,
            source.Code.Value,
            source.Name,
            source.Description,
            LevelId: source.LevelId,
            LevelCode: source.Level.Code.Value,
            LevelName: source.Level.Name,
            LevelTone: source.Level.Tone,
            CourseId: source.CourseId,
            CourseCode: string.Empty,
            CourseName: string.Empty,
            Teacher: new TeacherDto(source.TeacherMemberId, string.Empty, string.Empty, null),
            source.RoomId,
            RoomLabel: null,
            source.Format,
            source.Platform,
            Schedule: null,
            UpcomingLessons: [],
            source.Capacity,
            MemberCount: source.Members.Count(m => m.ExitedAt is null),
            source.Status,
            source.StartDate,
            source.EndDate
        );
}
