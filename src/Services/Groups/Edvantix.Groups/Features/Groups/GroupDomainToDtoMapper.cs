using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Groups.Features.Groups;

/// <summary>Маппер Group → GroupListItemDto. Обогащение курсом производится в обработчике.</summary>
public sealed class GroupListItemDtoMapper : Mapper<Group, GroupListItemDto>
{
    public override GroupListItemDto Map(Group source) =>
        new(
            source.Id,
            source.Code.Value,
            source.Name,
            source.Status,
            source.Format,
            source.Capacity,
            source.LevelId,
            source.Level.Code.Value,
            source.Level.Name,
            source.StartDate,
            source.EndDate,
            CourseCode: null,
            CourseName: null
        );
}

/// <summary>
/// Маппер Group → GroupDetailDto.
/// Поля курса, расписания и преподавателя заполняются в обработчике через fan-out.
/// </summary>
public sealed class GroupDetailDtoMapper : Mapper<Group, GroupDetailDto>
{
    public override GroupDetailDto Map(Group source) =>
        new(
            source.Id,
            source.Code.Value,
            source.Name,
            source.Description,
            source.Status,
            source.Format,
            source.Capacity,
            source.LevelId,
            source.Level.Code.Value,
            source.Level.Name,
            source.CourseId,
            CourseCode: null,
            CourseName: null,
            source.TeacherMemberId,
            Teacher: new TeacherDto(source.TeacherMemberId, string.Empty, null),
            source.RoomId,
            source.Platform,
            source.StartDate,
            source.EndDate,
            Schedule: null,
            UpcomingLessons: []
        );
}
