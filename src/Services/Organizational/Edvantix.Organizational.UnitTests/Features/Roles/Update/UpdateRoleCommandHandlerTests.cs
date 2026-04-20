using Edvantix.Chassis.Security.Tenant;

namespace Edvantix.Organizational.UnitTests.Features.Roles.Update;

public sealed class UpdateRoleCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRoleRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly UpdateRoleCommandHandler _handler;

    public UpdateRoleCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingRole_WhenUpdating_ThenShouldUpdateAndSave()
    {
        var role = CreateRole(_organizationId);
        var command = new UpdateRoleCommand(role.Id, "senior-manager", "Старший менеджер");

        _repoMock
            .Setup(r => r.GetByIdAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        role.Code.ShouldBe("senior-manager");
        role.Description.ShouldBe("Старший менеджер");
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenRoleNotFound_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var roleId = Guid.CreateVersion7();
        var command = new UpdateRoleCommand(roleId, "admin", null);

        _repoMock
            .Setup(r => r.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationMemberRole?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenRoleFromDifferentOrganization_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var role = CreateRole(Guid.CreateVersion7());
        var command = new UpdateRoleCommand(role.Id, "admin", null);

        _repoMock
            .Setup(r => r.GetByIdAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenExistingRole_WhenUpdatingWithNullDescription_ThenDescriptionShouldBeNull()
    {
        var role = CreateRole(_organizationId);
        var command = new UpdateRoleCommand(role.Id, "viewer", null);

        _repoMock
            .Setup(r => r.GetByIdAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        role.Description.ShouldBeNull();
    }

    private static OrganizationMemberRole CreateRole(Guid orgId) =>
        new(orgId, "manager", "Менеджер");
}
