using Edvantix.Constants.Other;
using Edvantix.Notification.Infrastructure.Senders.InApp;

namespace Edvantix.Notification.IntegrationEvents.EventHandlers;

internal sealed class SendInAppNotificationIntegrationEventHandler(IInAppSender sender)
{
    public async Task Handle(
        SendInAppNotificationIntegrationEvent @event,
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
