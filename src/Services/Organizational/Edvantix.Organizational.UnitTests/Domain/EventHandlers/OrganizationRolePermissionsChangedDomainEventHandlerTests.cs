using Edvantix.Chassis.Caching;

namespace Edvantix.Organizational.UnitTests.Domain.EventHandlers;

public sealed class OrganizationRolePermissionsChangedDomainEventHandlerTests
{
    private readonly Mock<IHybridCache> _cacheMock = new();
    private readonly OrganizationRolePermissionsChangedDomainEventHandler _handler;

    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid RoleId = Guid.CreateVersion7();

    public OrganizationRolePermissionsChangedDomainEventHandlerTests()
    {
        _cacheMock
            .Setup(c => c.RemoveByTagAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        _handler = new(_cacheMock.Object);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldRemoveByRolePermsTag()
    {
        var @event = new OrganizationRolePermissionsChangedDomainEvent(OrgId, RoleId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c => c.RemoveByTagAsync($"role-perms:{RoleId}", It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldNotRemoveByOtherTags()
    {
        var @event = new OrganizationRolePermissionsChangedDomainEvent(OrgId, RoleId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c =>
                c.RemoveByTagAsync(
                    It.Is<string>(t => t != $"role-perms:{RoleId}"),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldNotCallRemoveAsync()
    {
        var @event = new OrganizationRolePermissionsChangedDomainEvent(OrgId, RoleId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenDifferentRole_WhenHandling_ThenShouldUseCorrectRoleTag()
    {
        var anotherRoleId = Guid.CreateVersion7();
        var @event = new OrganizationRolePermissionsChangedDomainEvent(OrgId, anotherRoleId);

        await _handler.Handle(@event, CancellationToken.None);

        _cacheMock.Verify(
            c => c.RemoveByTagAsync($"role-perms:{anotherRoleId}", It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
