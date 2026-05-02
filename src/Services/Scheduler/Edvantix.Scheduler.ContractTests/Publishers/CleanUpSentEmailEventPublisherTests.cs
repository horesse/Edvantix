using Edvantix.Common;
using Edvantix.Contracts;

namespace Edvantix.Scheduler.ContractTests.Publishers;

public sealed class CleanUpSentEmailEventPublisherTests
{
    [Test]
    public async Task GivenCleanUpSentEmailIntegrationEvent_WhenPublished_ThenShouldMatchContract()
    {
        var @event = new CleanUpSentEmailIntegrationEvent();

        await SnapshotTestHelper.Verify(@event);
    }
}
