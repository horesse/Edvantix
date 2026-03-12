using Edvantix.Constants.Other;
using Edvantix.Notification.Infrastructure.Senders.InApp;

namespace Edvantix.Notification.IntegrationEvents.EventHandlers;

internal sealed class SendInAppNotificationIntegrationEventHandler(
    ILogger<SendInAppNotificationIntegrationEventHandler> logger,
    GlobalLogBuffer logBuffer,
    IInAppSender sender
)
{
    public async Task Consume(
        SendInAppNotificationIntegrationEvent @event,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var message = new InAppNotificationMessage
            {
                AccountId = @event.AccountId,
                Type = (NotificationType)@event.Type,
                Title = @event.Title,
                Message = @event.MessageText,
                Metadata = @event.Metadata,
            };

            await sender.SendAsync(message, cancellationToken);

            logger.LogInformation(
                "In-app notification created for account {AccountId} via event {EventId}",
                @event.AccountId,
                @event.Id
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to process SendInAppNotification event {EventId} for account {AccountId}",
                @event.Id,
                @event.AccountId
            );
            logBuffer.Flush();
            throw;
        }
    }
}
