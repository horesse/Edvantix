using Edvantix.Chassis.CQRS;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;
using Edvantix.Schedule.Domain.Enums;
using Edvantix.Schedule.Features.GroupSchedules;

namespace Edvantix.Schedule.Features.GroupSchedules.UpdateSettings;

[Transactional]
public sealed record UpdateGroupScheduleSettingsCommand(
    Guid GroupId,
    RecurrenceType Recurrence,
    short LessonDurationMinutes,
    EndMode EndMode,
    DateOnly? EndDate,
    short? LessonCount,
    int? BiweeklyParity,
    bool SkipHolidays,
    bool NotifyStudents,
    IReadOnlyList<SlotRequest> Slots
) : ICommand;

internal sealed class UpdateGroupScheduleSettingsCommandHandler(
    IGroupScheduleRepository repository
) : ICommandHandler<UpdateGroupScheduleSettingsCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateGroupScheduleSettingsCommand command,
        CancellationToken cancellationToken
    )
    {
        var schedule =
            await repository.GetByGroupIdAsync(command.GroupId, cancellationToken)
            ?? throw NotFoundException.For<GroupSchedule>(command.GroupId);

        schedule.UpdateSettings(
            command.Recurrence,
            command.LessonDurationMinutes,
            command.EndMode,
            command.EndDate,
            command.LessonCount,
            command.BiweeklyParity,
            command.SkipHolidays,
            command.NotifyStudents
        );

        schedule.ReplaceSlots(command.Slots.Select(s => (s.Weekday, s.StartMinutes)));

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Unit.Value;
    }
}
