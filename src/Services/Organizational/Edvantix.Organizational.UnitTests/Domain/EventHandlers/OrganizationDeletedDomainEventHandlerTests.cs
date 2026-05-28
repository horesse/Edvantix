using Edvantix.Organizational.Domain.EventHandlers.OrganizationEventHandlers;

namespace Edvantix.Organizational.UnitTests.Domain.EventHandlers;

public sealed class OrganizationDeletedDomainEventHandlerTests
{
    private readonly Mock<IFusionCache> _cacheMock = new();
    private readonly RemoveCacheOnDelete _handler;

    public OrganizationDeletedDomainEventHandlerTests()
    {
        _cacheMock
            .Setup(c =>
                c.RemoveAsync(
                    It.IsAny<string>(),
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(ValueTask.CompletedTask);

        _handler = new(_cacheMock.Object);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldRemoveCacheEntryWithCorrectKey()
    {
        var orgId = Guid.CreateVersion7();
        var @event = new OrganizationDeletedDomainEvent(orgId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c =>
                c.RemoveAsync(
                    $"organization:{orgId}",
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldNotRemoveOtherCacheEntries()
    {
        var orgId = Guid.CreateVersion7();
        var @event = new OrganizationDeletedDomainEvent(orgId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c =>
                c.RemoveAsync(
                    It.Is<string>(k => k != $"organization:{orgId}"),
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }
}
