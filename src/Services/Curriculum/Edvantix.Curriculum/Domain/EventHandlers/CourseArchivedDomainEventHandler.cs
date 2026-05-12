using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Curriculum.Domain.Events;

namespace Edvantix.Curriculum.Domain.EventHandlers;

/// <summary>
/// Публикует интеграционное событие при архивации курса,
/// чтобы Organizational-сервис мог пометить привязанные группы.
/// </summary>
internal sealed class CourseArchivedDomainEventHandler(IEventDispatcher dispatcher)
    : INotificationHandler<CourseArchivedDomainEvent>
{
    public async ValueTask Handle(
        CourseArchivedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        await dispatcher.DispatchAsync(notification, cancellationToken);
    }
}
