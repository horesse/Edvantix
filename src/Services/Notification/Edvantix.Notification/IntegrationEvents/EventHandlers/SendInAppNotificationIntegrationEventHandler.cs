using Edvantix.Constants.Other;
using Edvantix.Notification.Infrastructure.Senders.InApp;

namespace Edvantix.Notification.IntegrationEvents.EventHandlers;

/// <summary>
/// Обрабатывает событие <see cref="SendInAppNotificationIntegrationEvent"/>:
/// создаёт in-app уведомление через <see cref="IInAppSender"/>.
/// </summary>
public sealed class SendInAppNotificationIntegrationEventHandler(
    ILogger<SendInAppNotificationIntegrationEventHandler> logger,
    GlobalLogBuffer logBuffer,
    IInAppSender sender
) : IConsumer<SendInAppNotificationIntegrationEvent>
{
    public async Task Consume(ConsumeContext<SendInAppNotificationIntegrationEvent> context)
    {
        try
        {
            var message = new InAppNotificationMessage
            {
                ProfileId = context.Message.ProfileId,
                Type = (NotificationType)context.Message.Type,
                Title = context.Message.Title,
                Message = context.Message.MessageText,
                Metadata = context.Message.Metadata,
            };

            await sender.SendAsync(message, context.CancellationToken);

            logger.LogInformation(
                "In-app notification created for profile {ProfileId} via event {EventId}",
                context.Message.ProfileId,
                context.Message.Id
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to process SendInAppNotification event {EventId} for profile {ProfileId}",
                context.Message.Id,
                context.Message.ProfileId
            );
            logBuffer.Flush();
            throw;
        }
    }
}

[ExcludeFromCodeCoverage]
public sealed class SendInAppNotificationIntegrationEventHandlerDefinition
    : ConsumerDefinition<SendInAppNotificationIntegrationEventHandler>
{
    public SendInAppNotificationIntegrationEventHandlerDefinition()
    {
        // Имя очереди — явное, по соглашению notification-*
        Endpoint(x => x.Name = "notification-send-inapp");
        ConcurrentMessageLimit = 10;
    }
}
