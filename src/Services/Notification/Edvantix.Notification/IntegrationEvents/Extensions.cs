using Edvantix.Constants.Other;
using Wolverine.Kafka;

namespace Edvantix.Notification.IntegrationEvents;

public static class Extensions
{
    extension(WolverineOptions options)
    {
        public void AddEvents()
        {
            options
                .ListenToKafkaTopic(Topics.Notification)
                .InteropWithCloudEvents()
                .MaximumParallelMessages(10);
        }
    }
}
