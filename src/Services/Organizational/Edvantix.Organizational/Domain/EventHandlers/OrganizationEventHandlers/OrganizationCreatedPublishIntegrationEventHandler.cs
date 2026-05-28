using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers.OrganizationEventHandlers;

/// <summary>
/// Публикует интеграционное событие о создании организации, чтобы Groups-сервис
/// мог автоматически сидировать дефолтные уровни.
/// </summary>
internal sealed class OrganizationCreatedPublishIntegrationEventHandler(IEventDispatcher dispatcher)
    : INotificationHandler<OrganizationCreatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationCreatedDomainEvent notification,
        CancellationToken cancellationToken
    ) => await dispatcher.DispatchAsync(notification, cancellationToken);
}
