using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;

/// <summary>Исключение (пропуск) для конкретной даты в расписании.</summary>
public sealed class ScheduleException : Entity
{
    private ScheduleException() { }

    /// <param name="scheduleId">Идентификатор родительского расписания.</param>
    /// <param name="exceptionDate">Дата пропуска.</param>
    /// <param name="reason">Необязательная причина пропуска.</param>
    public ScheduleException(Guid scheduleId, DateOnly exceptionDate, string? reason = null)
    {
        Id = Guid.CreateVersion7();
        ScheduleId = scheduleId;
        ExceptionDate = exceptionDate;
        Reason = reason?.Trim();
    }

    public Guid ScheduleId { get; private set; }
    public DateOnly ExceptionDate { get; private set; }
    public string? Reason { get; private set; }
}
