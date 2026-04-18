using Edvantix.Chassis.Caching;

namespace Edvantix.Organizational.UnitTests.Domain.EventHandlers;

public sealed class OrganizationMemberCreatedDomainEventHandlerTests
{
    private readonly Mock<IHybridCache> _cacheMock = new();
    private readonly OrganizationMemberCreatedDomainEventHandler _handler;

    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid ProfileId = Guid.CreateVersion7();

    public OrganizationMemberCreatedDomainEventHandlerTests()
    {
        _cacheMock
            .Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        _handler = new(_cacheMock.Object);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldRemoveCacheEntryWithCorrectKey()
    {
        var @event = new OrganizationMemberCreatedDomainEvent(OrgId, ProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c =>
                c.RemoveAsync(
                    $"perm:org:{OrgId}:profile:{ProfileId}",
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldNotRemoveOtherCacheEntries()
    {
        var @event = new OrganizationMemberCreatedDomainEvent(OrgId, ProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c =>
                c.RemoveAsync(
                    It.Is<string>(k => k != $"perm:org:{OrgId}:profile:{ProfileId}"),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Test]
    public async Task GivenDifferentOrgAndProfile_WhenHandling_ThenShouldUseCorrectKey()
    {
        var anotherOrgId = Guid.CreateVersion7();
        var anotherProfileId = Guid.CreateVersion7();
        var @event = new OrganizationMemberCreatedDomainEvent(anotherOrgId, anotherProfileId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c =>
                c.RemoveAsync(
                    $"perm:org:{anotherOrgId}:profile:{anotherProfileId}",
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}
