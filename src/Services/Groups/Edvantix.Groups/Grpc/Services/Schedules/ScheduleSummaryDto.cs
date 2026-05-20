namespace Edvantix.Groups.Grpc.Services.Schedules;

/// <summary>Сводка расписания группы, полученная из Schedule-сервиса.</summary>
public sealed record ScheduleSummaryDto(
    string SummaryText,
    int LessonDurationMinutes,
    DateOnly? NextLessonDate,
    int LessonCountTotal,
    int LessonCountRemaining
);
