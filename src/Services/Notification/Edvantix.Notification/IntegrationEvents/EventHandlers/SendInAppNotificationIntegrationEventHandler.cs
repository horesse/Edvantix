using Edvantix.Constants.Other;
using Edvantix.Notification.Infrastructure.Senders.InApp;

namespace Edvantix.Notification.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает событие <see cref="SendInAppNotificationIntegrationEvent"/>:
/// создаёт in-app уведомление через <see cref="IInAppSender"/>.
/// </summary>
public static class SendInAppNotificationIntegrationEventHandler
{
    public static async Task Handle(
        SendInAppNotificationIntegrationEvent @event,
        IInAppSender sender,
        CancellationToken cancellationToken
    )
    {
        var message = new InAppNotificationMessage
        {
            ProfileId = @event.ProfileId,
            Type = (NotificationType)@event.Type,
            Title = @event.Title,
            Message = @event.MessageText,
            Metadata = @event.Metadata,
        };

        await sender.SendAsync(message, cancellationToken);
    }
}
