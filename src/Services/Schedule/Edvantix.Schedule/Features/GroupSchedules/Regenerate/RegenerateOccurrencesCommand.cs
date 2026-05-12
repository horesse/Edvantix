using Edvantix.Chassis.CQRS;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;
using Edvantix.Schedule.Domain.AggregatesModel.HolidayAggregate;
using Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;

namespace Edvantix.Schedule.Features.GroupSchedules.Regenerate;

/// <summary>
/// Удаляет все запланированные занятия и материализует расписание заново.
/// Используется после изменения настроек расписания.
/// </summary>
[Transactional]
public sealed record RegenerateOccurrencesCommand(
    Guid GroupId,
    string? HolidayCountryCode
) : ICommand;

internal sealed class RegenerateOccurrencesCommandHandler(
    IGroupScheduleRepository scheduleRepository,
    ILessonOccurrenceRepository occurrenceRepository,
    IHolidayRepository holidayRepository
) : ICommandHandler<RegenerateOccurrencesCommand>
{
    public async ValueTask<Unit> Handle(
        RegenerateOccurrencesCommand command,
        CancellationToken cancellationToken
    )
    {
        var schedule =
            await scheduleRepository.GetByGroupIdAsync(command.GroupId, cancellationToken)
            ?? throw NotFoundException.For<GroupSchedule>(command.GroupId);

        await occurrenceRepository.DeleteByScheduleIdAsync(schedule.Id, cancellationToken);

        if (schedule.Slots.Count == 0)
        {
            await scheduleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            return Unit.Value;
        }

        var holidays = command is { HolidayCountryCode: not null } && schedule.SkipHolidays
            ? await LoadHolidays(command.HolidayCountryCode, schedule.StartDate.Year, cancellationToken)
            : [];

        var occurrences = schedule.Materialize(holidays);
        if (occurrences.Count > 0)
            await occurrenceRepository.AddRangeAsync(occurrences, cancellationToken);

        await scheduleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Unit.Value;
    }

    private async Task<IReadOnlyList<Holiday>> LoadHolidays(
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
