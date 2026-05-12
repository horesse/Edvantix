using Edvantix.Schedule.Domain.Enums;

namespace Edvantix.Schedule.Features.GroupSchedules;

/// <summary>DTO расписания группы для API.</summary>
public sealed record GroupScheduleDto(
    Guid Id,
    Guid GroupId,
    Guid OrganizationId,
    RecurrenceType Recurrence,
    int? BiweeklyParity,
    short LessonDurationMinutes,
    DateOnly StartDate,
    EndMode EndMode,
    DateOnly? EndDate,
    short? LessonCount,
    bool SkipHolidays,
    bool NotifyStudents,
    IReadOnlyList<SlotDto> Slots,
    IReadOnlyList<ExceptionDto> Exceptions
);

public sealed record SlotDto(int Weekday, int StartMinutes);

public sealed record ExceptionDto(DateOnly Date, string? Reason);
