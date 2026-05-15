namespace Edvantix.Organizational.Grpc.Services.Schedules;

/// <summary>Сводка расписания группы, полученная из Schedule-сервиса.</summary>
/// <param name="SummaryText">Человекочитаемое расписание, например "Пн / Ср · 18:00–19:30".</param>
/// <param name="LessonDurationMinutes">Длительность занятия в минутах.</param>
/// <param name="NextLessonDate">Дата ближайшего занятия; <c>null</c> если нет.</param>
/// <param name="LessonCountTotal">Всего материализованных занятий.</param>
/// <param name="LessonCountRemaining">Число предстоящих занятий.</param>
public sealed record ScheduleSummaryDto(
    string SummaryText,
    int LessonDurationMinutes,
    DateOnly? NextLessonDate,
    int LessonCountTotal,
    int LessonCountRemaining
);
