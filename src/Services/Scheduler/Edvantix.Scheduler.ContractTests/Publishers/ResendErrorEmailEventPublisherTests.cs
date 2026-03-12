using Edvantix.Common;
using Edvantix.Contracts;

namespace Edvantix.Scheduler.ContractTests.Publishers;

public sealed class ResendErrorEmailEventPublisherTests
{
    [Test]
    public async Task GivenResendErrorEmailIntegrationEvent_WhenPublished_ThenShouldMatchContract()
    {
        // Arrange
        var @event = new ResendErrorEmailIntegrationEvent();

        // Assert
        await SnapshotTestHelper.Verify(@event);
    }
}
