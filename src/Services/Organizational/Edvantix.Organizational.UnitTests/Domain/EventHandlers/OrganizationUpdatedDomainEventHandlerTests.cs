namespace Edvantix.Organizational.UnitTests.Domain.EventHandlers;

public sealed class OrganizationUpdatedDomainEventHandlerTests
{
    private readonly Mock<IFusionCache> _cacheMock = new();
    private readonly OrganizationUpdatedDomainEventHandler _handler;

    public OrganizationUpdatedDomainEventHandlerTests()
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
    public async Task GivenValidEvent_WhenHandling_ThenShouldRemoveDetailCacheEntry()
    {
        var orgId = Guid.CreateVersion7();
        var @event = new OrganizationUpdatedDomainEvent(orgId);

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
    public async Task GivenValidEvent_WhenHandling_ThenShouldRemoveSummaryCacheEntry()
    {
        var orgId = Guid.CreateVersion7();
        var @event = new OrganizationUpdatedDomainEvent(orgId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c =>
                c.RemoveAsync(
                    $"org:{orgId}:summary",
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldRemoveExactlyTwoCacheEntries()
    {
        var orgId = Guid.CreateVersion7();
        var @event = new OrganizationUpdatedDomainEvent(orgId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c =>
                c.RemoveAsync(
                    It.IsAny<string>(),
                    It.IsAny<FusionCacheEntryOptions?>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Exactly(2)
        );
    }
}
