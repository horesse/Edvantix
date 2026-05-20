using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Groups.Domain.Events;

namespace Edvantix.Groups.Domain.EventHandlers;

/// <summary>
/// Публикует интеграционное событие о создании группы, чтобы Schedule-сервис
/// мог автоматически создать пустое расписание.
/// </summary>
internal sealed class GroupCreatedDomainEventHandler(IEventDispatcher dispatcher)
    : INotificationHandler<GroupCreatedDomainEvent>
{
    public async ValueTask Handle(
        GroupCreatedDomainEvent notification,
        CancellationToken cancellationToken
    ) => await dispatcher.DispatchAsync(notification, cancellationToken);
}
