using Edvantix.Chassis.Mapper;
using Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;

namespace Edvantix.Schedule.Features.LessonOccurrences;

internal sealed class DomainToDtoMapper : Mapper<LessonOccurrence, LessonOccurrenceDto>
{
    public override LessonOccurrenceDto Map(LessonOccurrence source) =>
        new(
            source.Id,
            source.ScheduleId,
            source.GroupId,
            source.LessonDate,
            source.StartMinutes,
            source.DurationMinutes,
            source.Status,
            source.SkipReason,
            source.LessonRefId
        );
}
