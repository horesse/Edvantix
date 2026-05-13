using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Organizational.Features.Groups;

/// <summary>Маппер Group → GroupListItemDto (без обогащения профилем и кабинетом).</summary>
public sealed class GroupListItemDtoMapper : Mapper<Group, GroupListItemDto>
{
    public override GroupListItemDto Map(Group source) =>
        new(
            source.Id,
            source.Code.Value,
            source.Name,
            source.Level,
            source.Format,
            source.Status,
            source.Capacity,
            source.Members.Count(m => m.ExitedAt is null),
            source.StartDate,
            source.EndDate,
            source.TeacherMemberId,
            TeacherFullName: string.Empty,
            source.RoomId,
            RoomLabel: null,
            source.CourseId
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
            source.Level,
            source.Format,
            source.Status,
            source.Capacity,
            source.Members.Count(m => m.ExitedAt is null),
            source.StartDate,
            source.EndDate,
            source.CourseId,
            source.TeacherMemberId,
            TeacherFullName: string.Empty,
            source.RoomId,
            RoomLabel: null,
            source.Platform
        );
}
