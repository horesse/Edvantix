using Edvantix.Constants.Other;
using Wolverine;
using Wolverine.Kafka;

namespace Edvantix.Identity.IntegrationEvents;

public static class Extensions
{
    extension(WolverineOptions options)
    {
        public void AddEvents()
        {
            options
                .ListenToKafkaTopic(Topics.Identity)
                .InteropWithCloudEvents()
                .MaximumParallelMessages(10);
        }
    }
}
