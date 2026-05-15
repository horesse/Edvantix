using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Features.OrganizationMembers;

namespace Edvantix.Organizational.Features.Groups;

/// <summary>Маппер Group → GroupListItemDto (без обогащения профилем и кабинетом).</summary>
public sealed class GroupListItemDtoMapper : Mapper<Group, GroupListItemDto>
{
    public override GroupListItemDto Map(Group source) =>
        new(
            source.Id,
            source.Code.Value,
            source.Name,
            source.LevelId,
            source.Format,
            source.Status,
            source.Capacity,
            source.Members.Count(m => m.ExitedAt is null),
            source.StartDate,
            source.EndDate,
            Teacher: new TeacherDto(source.TeacherMemberId, string.Empty, string.Empty, null),
            source.RoomId,
            RoomLabel: null,
            source.CourseId,
            ScheduleSummary: null
        );
}

/// <summary>Маппер Group → GroupDetailDto (без обогащения профилем и кабинетом).</summary>
public sealed class GroupDetailDtoMapper : Mapper<Group, GroupDetailDto>
{
    public override GroupDetailDto Map(Group source) =>
        new(
            source.Id,
            source.Code.Value,
            source.Name,
            source.Description,
            source.LevelId,
            source.Format,
            source.Status,
            source.Capacity,
            source.Members.Count(m => m.ExitedAt is null),
            source.StartDate,
            source.EndDate,
            source.CourseId,
            Teacher: new TeacherDto(source.TeacherMemberId, string.Empty, string.Empty, null),
            source.RoomId,
            RoomLabel: null,
            source.Platform
        );
}
