using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Contracts;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Отправляет уведомление о приглашении сразу после его создания.
/// Email-приглашения — через интеграционное событие <see cref="SendEmailInvitationIntegrationEvent"/>;
/// In-app — через <see cref="SendInAppNotificationIntegrationEvent"/>.
/// </summary>
internal sealed class InvitationCreatedDomainEventHandler(IEventDispatcher dispatcher)
    : INotificationHandler<InvitationCreatedDomainEvent>
{
    public async ValueTask Handle(
        InvitationCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        await dispatcher.DispatchAsync(notification, cancellationToken);
    }
}
