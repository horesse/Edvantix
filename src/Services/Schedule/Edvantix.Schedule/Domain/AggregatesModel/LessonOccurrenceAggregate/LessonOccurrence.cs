using Edvantix.Schedule.Domain.Enums;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;

/// <summary>
/// Материализованное занятие — конкретная дата/время проведения урока.
/// Генерируется из <c>GroupSchedule.Materialize()</c> и пересоздаётся при изменении расписания.
/// </summary>
public sealed class LessonOccurrence : Entity, IAggregateRoot
{
    private LessonOccurrence() { }

    /// <param name="scheduleId">Идентификатор расписания-источника.</param>
    /// <param name="groupId">Идентификатор группы (логическая FK).</param>
    /// <param name="lessonDate">Дата занятия.</param>
    /// <param name="startMinutes">Начало в минутах от полуночи.</param>
    /// <param name="durationMinutes">Длительность занятия в минутах.</param>
    public LessonOccurrence(
        Guid scheduleId,
        Guid groupId,
        DateOnly lessonDate,
        int startMinutes,
        short durationMinutes
    )
    {
        Id = Guid.CreateVersion7();
        ScheduleId = scheduleId;
        GroupId = groupId;
        LessonDate = lessonDate;
        StartMinutes = startMinutes;
        DurationMinutes = durationMinutes;
        Status = OccurrenceStatus.Planned;
    }

    public Guid ScheduleId { get; private set; }
    public Guid GroupId { get; private set; }

    /// <summary>Дата проведения занятия.</summary>
    public DateOnly LessonDate { get; private set; }

    /// <summary>Начало занятия в минутах от полуночи.</summary>
    public int StartMinutes { get; private set; }

    /// <summary>Длительность занятия в минутах.</summary>
    public short DurationMinutes { get; private set; }

    /// <summary>Текущий статус занятия.</summary>
    public OccurrenceStatus Status { get; private set; }

    /// <summary>Причина пропуска/отмены.</summary>
    public string? SkipReason { get; private set; }

    /// <summary>
    /// Опциональная ссылка на урок из Curriculum-сервиса (логическая FK).
    /// </summary>
    public Guid? LessonRefId { get; private set; }

    /// <summary>Помечает занятие как проведённое.</summary>
    public void MarkAsHeld() => Status = OccurrenceStatus.Held;

    /// <summary>Помечает занятие как пропущенное.</summary>
    public void MarkAsSkipped(string? reason = null)
    {
        Status = OccurrenceStatus.Skipped;
        SkipReason = reason?.Trim();
    }

    /// <summary>Отменяет занятие.</summary>
    public void Cancel(string? reason = null)
    {
        Status = OccurrenceStatus.Cancelled;
        SkipReason = reason?.Trim();
    }
}
