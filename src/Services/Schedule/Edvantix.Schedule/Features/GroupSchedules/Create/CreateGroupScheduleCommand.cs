using Edvantix.Chassis.CQRS;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;
using Edvantix.Schedule.Domain.AggregatesModel.HolidayAggregate;
using Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;
using Edvantix.Schedule.Domain.Enums;
using Edvantix.Schedule.Features.GroupSchedules;

namespace Edvantix.Schedule.Features.GroupSchedules.Create;

/// <summary>
/// Создаёт расписание для группы и сразу материализует занятия.
/// </summary>
[Transactional]
public sealed record CreateGroupScheduleCommand(
    Guid GroupId,
    Guid OrganizationId,
    RecurrenceType Recurrence,
    short LessonDurationMinutes,
    DateOnly StartDate,
    EndMode EndMode,
    DateOnly? EndDate,
    short? LessonCount,
    int? BiweeklyParity,
    bool SkipHolidays,
    bool NotifyStudents,
    IReadOnlyList<SlotRequest> Slots,
    string? HolidayCountryCode
) : ICommand<Guid>;

internal sealed class CreateGroupScheduleCommandHandler(
    IGroupScheduleRepository scheduleRepository,
    ILessonOccurrenceRepository occurrenceRepository,
    IHolidayRepository holidayRepository
) : ICommandHandler<CreateGroupScheduleCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateGroupScheduleCommand command,
        CancellationToken cancellationToken
    )
    {
        var existing = await scheduleRepository.GetByGroupIdAsync(
            command.GroupId,
            cancellationToken
        );

        if (existing is not null)
            throw new InvalidOperationException(
                $"Расписание для группы {command.GroupId} уже существует."
            );

        var schedule = new GroupSchedule(
            command.GroupId,
            command.OrganizationId,
            command.Recurrence,
            command.LessonDurationMinutes,
            command.StartDate,
            command.EndMode,
            command.EndDate,
            command.LessonCount,
            command.BiweeklyParity,
            command.SkipHolidays,
            command.NotifyStudents
        );

        if (command.Slots.Count > 0)
            schedule.ReplaceSlots(command.Slots.Select(s => (s.Weekday, s.StartMinutes)));

        await scheduleRepository.AddAsync(schedule, cancellationToken);

        // Материализуем занятия если есть слоты
        if (schedule.Slots.Count > 0)
        {
            var holidays = command is { SkipHolidays: true, HolidayCountryCode: not null }
                ? await LoadHolidays(command.HolidayCountryCode, command.StartDate.Year, cancellationToken)
                : [];

            var occurrences = schedule.Materialize(holidays);
            if (occurrences.Count > 0)
                await occurrenceRepository.AddRangeAsync(occurrences, cancellationToken);
        }

        await scheduleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return schedule.Id;
    }

    private async Task<IReadOnlyList<Domain.AggregatesModel.HolidayAggregate.Holiday>> LoadHolidays(
        string countryCode,
        int year,
        CancellationToken cancellationToken
    )
    {
        var current = await holidayRepository.GetByCountryAndYearAsync(
            countryCode,
            year,
            cancellationToken
        );
        var next = await holidayRepository.GetByCountryAndYearAsync(
            countryCode,
            year + 1,
            cancellationToken
        );
        return [..current, ..next];
    }
}
