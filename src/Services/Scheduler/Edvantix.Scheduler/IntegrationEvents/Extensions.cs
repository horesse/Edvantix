using Edvantix.Constants.Other;
using Edvantix.Contracts;
using Wolverine;
using Wolverine.Kafka;

namespace Edvantix.Scheduler.IntegrationEvents;

public static class Extensions
{
    extension(WolverineOptions options)
    {
        public void AddEvents()
        {
            options
                .PublishMessage<CleanUpSentEmailIntegrationEvent>()
                .ToKafkaTopic(Topics.Notification)
                .InteropWithCloudEvents();

            options
                .PublishMessage<ResendErrorEmailIntegrationEvent>()
                .ToKafkaTopic(Topics.Notification)
                .InteropWithCloudEvents();
        }
    }
}
