using Edvantix.Constants.Other;
using Edvantix.Contracts;
using Wolverine.Kafka;

namespace Edvantix.Persona.IntegrationEvents;

public static class Extensions
{
    extension(WolverineOptions options)
    {
        public void AddEvents()
        {
            options
                .PublishMessage<SendInAppNotificationIntegrationEvent>()
                .ToKafkaTopic(Topics.Notification)
                .InteropWithCloudEvents();

            options
                .PublishMessage<EnableKeycloakUserIntegrationEvent>()
                .ToKafkaTopic(Topics.Identity)
                .InteropWithCloudEvents();

            options
                .PublishMessage<DisableKeycloakUserIntegrationEvent>()
                .ToKafkaTopic(Topics.Identity)
                .InteropWithCloudEvents();

            options
                .PublishMessage<LinkKeycloakProfileIntegrationEvent>()
                .ToKafkaTopic(Topics.Identity)
                .InteropWithCloudEvents();
        }
    }
}
