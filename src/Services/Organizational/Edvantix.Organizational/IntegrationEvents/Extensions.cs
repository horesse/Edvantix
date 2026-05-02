using Edvantix.Constants.Other;
using Edvantix.Contracts;
using Wolverine.Kafka;

namespace Edvantix.Organizational.IntegrationEvents;

public static class Extensions
{
    extension(WolverineOptions options)
    {
        public void AddEvents()
        {
            options
                .PublishMessage<SendEmailInvitationIntegrationEvent>()
                .ToKafkaTopic(Topics.Notification)
                .InteropWithCloudEvents();

            options
                .PublishMessage<SendInAppNotificationIntegrationEvent>()
                .ToKafkaTopic(Topics.Notification)
                .InteropWithCloudEvents();
        }
    }
}
