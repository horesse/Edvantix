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
        ILogger logger,
        GlobalLogBuffer logBuffer,
        IInAppSender sender,
        CancellationToken cancellationToken
    )
    {
        try
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

            logger.LogInformation(
                "In-app notification created for profile {ProfileId} via event {EventId}",
                @event.ProfileId,
                @event.Id
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to process SendInAppNotification event {EventId} for profile {ProfileId}",
                @event.Id,
                @event.ProfileId
            );
            logBuffer.Flush();
            throw;
        }
    }
}
