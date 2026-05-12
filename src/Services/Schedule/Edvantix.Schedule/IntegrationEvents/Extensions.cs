using Edvantix.Constants.Other;
using Wolverine;
using Wolverine.Kafka;

namespace Edvantix.Schedule.IntegrationEvents;

public static class Extensions
{
    extension(WolverineOptions options)
    {
        public void AddEvents()
        {
            options
                .ListenToKafkaTopic(Topics.Schedule)
                .InteropWithCloudEvents()
                .MaximumParallelMessages(5);
        }
    }
}
