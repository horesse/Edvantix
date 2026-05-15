namespace Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;

/// <summary>Агрегированные счётчики занятий одной группы.</summary>
/// <param name="Total">Всего материализованных занятий.</param>
/// <param name="Remaining">Предстоящих занятий (дата ≥ сегодня).</param>
/// <param name="NextLessonDate">Дата ближайшего занятия; <c>null</c> если нет.</param>
public sealed record OccurrenceSummary(int Total, int Remaining, DateOnly? NextLessonDate);
