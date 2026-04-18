using Edvantix.Chassis.Caching;

namespace Edvantix.Organizational.UnitTests.Domain.EventHandlers;

public sealed class OrganizationRolePermissionsChangedDomainEventHandlerTests
{
    private readonly Mock<IHybridCache> _cacheMock = new();
    private readonly OrganizationRolePermissionsChangedDomainEventHandler _handler;

    private static readonly Guid OrgId = Guid.CreateVersion7();

    public OrganizationRolePermissionsChangedDomainEventHandlerTests()
    {
        _cacheMock
            .Setup(c => c.RemoveByTagAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        _handler = new(_cacheMock.Object);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldRemoveByTagWithCorrectTag()
    {
        var @event = new OrganizationRolePermissionsChangedDomainEvent(OrgId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c => c.RemoveByTagAsync($"org-perms:{OrgId}", It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldNotRemoveByOtherTags()
    {
        var @event = new OrganizationRolePermissionsChangedDomainEvent(OrgId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c =>
                c.RemoveByTagAsync(
                    It.Is<string>(t => t != $"org-perms:{OrgId}"),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldNotCallRemoveAsync()
    {
        var @event = new OrganizationRolePermissionsChangedDomainEvent(OrgId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenDifferentOrg_WhenHandling_ThenShouldUseCorrectTag()
    {
        var anotherOrgId = Guid.CreateVersion7();
        var @event = new OrganizationRolePermissionsChangedDomainEvent(anotherOrgId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c => c.RemoveByTagAsync($"org-perms:{anotherOrgId}", It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
