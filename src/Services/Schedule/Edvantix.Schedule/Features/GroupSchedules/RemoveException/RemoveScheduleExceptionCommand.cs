using Edvantix.Chassis.CQRS;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;

namespace Edvantix.Schedule.Features.GroupSchedules.RemoveException;

[Transactional]
public sealed record RemoveScheduleExceptionCommand(Guid GroupId, Guid ExceptionId) : ICommand;

internal sealed class RemoveScheduleExceptionCommandHandler(
    IGroupScheduleRepository repository
) : ICommandHandler<RemoveScheduleExceptionCommand>
{
    public async ValueTask<Unit> Handle(
        RemoveScheduleExceptionCommand command,
        CancellationToken cancellationToken
    )
    {
        var schedule =
            await repository.GetByGroupIdAsync(command.GroupId, cancellationToken)
            ?? throw NotFoundException.For<GroupSchedule>(command.GroupId);

        schedule.RemoveException(command.ExceptionId);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Unit.Value;
    }
}
