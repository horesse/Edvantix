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
            // LevelCode и LevelName недоступны из домена — Level хранится в Organizational-сервисе
            // как мягкая ссылка. Обогащение производится в обработчике через cross-service вызов.
            LevelCode: string.Empty,
            LevelName: string.Empty,
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
            // LevelCode и LevelName недоступны из домена — Level хранится в Organizational-сервисе
            // как мягкая ссылка. Обогащение производится в обработчике через cross-service вызов.
            LevelCode: string.Empty,
            LevelName: string.Empty,
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
