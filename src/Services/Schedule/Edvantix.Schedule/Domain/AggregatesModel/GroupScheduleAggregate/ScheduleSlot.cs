using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;

/// <summary>
/// Слот в недельной сетке расписания.
/// <para>Неизменяемая value-like сущность: изменение слота = удалить старый + создать новый.</para>
/// </summary>
public sealed class ScheduleSlot : Entity
{
    private ScheduleSlot() { }

    /// <param name="scheduleId">Идентификатор родительского расписания.</param>
    /// <param name="weekday">День недели (0 = воскресенье … 6 = суббота).</param>
    /// <param name="startMinutes">Начало занятия в минутах от полуночи (0–1439).</param>
    public ScheduleSlot(Guid scheduleId, int weekday, int startMinutes)
    {
        if (weekday is < 0 or > 6)
            throw new ArgumentOutOfRangeException(
                nameof(weekday),
                "День недели должен быть от 0 до 6."
            );

        if (startMinutes is < 0 or > 1439)
            throw new ArgumentOutOfRangeException(
                nameof(startMinutes),
                "Минуты начала должны быть от 0 до 1439."
            );

        Id = Guid.CreateVersion7();
        ScheduleId = scheduleId;
        Weekday = weekday;
        StartMinutes = startMinutes;
    }

    /// <summary>Идентификатор расписания.</summary>
    public Guid ScheduleId { get; private set; }

    /// <summary>День недели (0 = воскресенье, 6 = суббота).</summary>
    public int Weekday { get; private set; }

    /// <summary>Начало занятия в минутах от полуночи (0–1439).</summary>
    public int StartMinutes { get; private set; }
}
