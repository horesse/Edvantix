using Edvantix.Schedule.Domain.Enums;

namespace Edvantix.Schedule.Features.LessonOccurrences;

public sealed record LessonOccurrenceDto(
    Guid Id,
    Guid ScheduleId,
    Guid GroupId,
    DateOnly LessonDate,
    int StartMinutes,
    int DurationMinutes,
    OccurrenceStatus Status,
    string? SkipReason,
    Guid? LessonRefId
);
