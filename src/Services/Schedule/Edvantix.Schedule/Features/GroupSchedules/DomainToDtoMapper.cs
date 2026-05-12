using Edvantix.Chassis.Mapper;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;

namespace Edvantix.Schedule.Features.GroupSchedules;

internal sealed class GroupScheduleDomainToDtoMapper : Mapper<GroupSchedule, GroupScheduleDto>
{
    public override GroupScheduleDto Map(GroupSchedule source) =>
        new(
            source.Id,
            source.GroupId,
            source.OrganizationId,
            source.Recurrence,
            source.BiweeklyParity,
            source.LessonDurationMinutes,
            source.StartDate,
            source.EndMode,
            source.EndDate,
            source.LessonCount,
            source.SkipHolidays,
            source.NotifyStudents,
            source.Slots.Select(s => new SlotDto(s.Weekday, s.StartMinutes)).ToList(),
            source.Exceptions.Select(e => new ExceptionDto(e.ExceptionDate, e.Reason)).ToList()
        );
}
