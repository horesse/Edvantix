using Edvantix.Contracts;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;

namespace Edvantix.Schedule.IntegrationEvents.EventHandlers;

/// <summary>
/// Создаёт пустое расписание при создании группы.
/// Слоты и параметры можно задать позже через API.
/// </summary>
public sealed class GroupCreatedIntegrationEventHandler(
    IGroupScheduleRepository repository
)
{
    public async Task Handle(
        GroupCreatedIntegrationEvent @event,
        CancellationToken cancellationToken
    )
    {
        var existing = await repository.GetByGroupIdAsync(@event.GroupId, cancellationToken);
        if (existing is not null)
            return;

        var schedule = GroupSchedule.CreateEmpty(
            @event.GroupId,
            @event.OrganizationId,
            @event.StartDate
        );

        await repository.AddAsync(schedule, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
