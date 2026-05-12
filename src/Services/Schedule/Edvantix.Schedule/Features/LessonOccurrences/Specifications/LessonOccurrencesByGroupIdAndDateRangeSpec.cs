using Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;

namespace Edvantix.Schedule.Features.LessonOccurrences.Specifications;

internal sealed class LessonOccurrencesByGroupIdAndDateRangeSpec : Specification<LessonOccurrence>
{
    public LessonOccurrencesByGroupIdAndDateRangeSpec(Guid groupId, DateOnly from, DateOnly to)
    {
        Query
            .Where(o => o.GroupId == groupId && o.LessonDate >= from && o.LessonDate <= to)
            .OrderBy(o => o.LessonDate)
            .ThenBy(o => o.StartMinutes);
    }
}
