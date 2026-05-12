using Edvantix.Chassis.CQRS;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;

namespace Edvantix.Schedule.Features.GroupSchedules.AddException;

[Transactional]
public sealed record AddScheduleExceptionCommand(
    Guid GroupId,
    DateOnly ExceptionDate,
    string? Reason
) : ICommand<Guid>;

internal sealed class AddScheduleExceptionCommandHandler(
    IGroupScheduleRepository repository
) : ICommandHandler<AddScheduleExceptionCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        AddScheduleExceptionCommand command,
        CancellationToken cancellationToken
    )
    {
        var schedule =
            await repository.GetByGroupIdAsync(command.GroupId, cancellationToken)
            ?? throw NotFoundException.For<GroupSchedule>(command.GroupId);

        var exception = schedule.AddException(command.ExceptionDate, command.Reason);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return exception.Id;
    }
}
