namespace Edvantix.Organizational.UnitTests.Features.Roles.Update;

public sealed class UpdateRoleCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRoleRepository> _repoMock = new();
    private readonly Mock<IPermissionRepository> _permRepoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly UpdateRoleCommandHandler _handler;

    public UpdateRoleCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _permRepoMock.Object);
    }

    [Test]
    public async Task GivenExistingRole_WhenUpdating_ThenShouldUpdateAndSave()
    {
        var role = CreateRole(_organizationId);
        var command = new UpdateRoleCommand(
            role.Id,
            "Старший менеджер",
            "Управление несколькими командами",
            []
        );

        SetupRepo(role);
        SetupPermRepo([]);

        await _handler.Handle(command, CancellationToken.None);

        role.Name.ShouldBe("Старший менеджер");
        role.Description.ShouldBe("Управление несколькими командами");
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenRoleNotFound_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var roleId = Guid.CreateVersion7();
        var command = new UpdateRoleCommand(roleId, "Администратор", null, []);

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
        var command = new UpdateRoleCommand(role.Id, "Администратор", null, []);

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
        var command = new UpdateRoleCommand(role.Id, "Читатель", null, []);

        SetupRepo(role);
        SetupPermRepo([]);

        await _handler.Handle(command, CancellationToken.None);

        role.Description.ShouldBeNull();
    }

    [Test]
    public async Task GivenPermissionIds_WhenUpdating_ThenShouldAssignPermissionsToRole()
    {
        var role = CreateRole(_organizationId);
        var perm1 = CreatePermission("View");
        var perm2 = CreatePermission("Edit");
        var command = new UpdateRoleCommand(
            role.Id,
            role.Name,
            role.Description,
            [perm1.Id, perm2.Id]
        );

        SetupRepo(role);
        SetupPermRepo([perm1, perm2]);

        await _handler.Handle(command, CancellationToken.None);

        role.Permissions.Count.ShouldBe(2);
        role.Permissions.ShouldContain(perm1);
        role.Permissions.ShouldContain(perm2);
    }

    [Test]
    public async Task GivenRoleWithExistingPermissions_WhenUpdatingWithEmptyPermissions_ThenShouldClearPermissions()
    {
        var role = CreateRole(_organizationId);
        role.AddPermission(CreatePermission("Delete"));
        var command = new UpdateRoleCommand(role.Id, role.Name, role.Description, []);

        SetupRepo(role);
        SetupPermRepo([]);

        await _handler.Handle(command, CancellationToken.None);

        role.Permissions.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenPermissionIds_WhenUpdating_ThenShouldLoadPermissionsFromRepository()
    {
        var role = CreateRole(_organizationId);
        var perm = CreatePermission("View");
        var command = new UpdateRoleCommand(role.Id, role.Name, role.Description, [perm.Id]);

        SetupRepo(role);
        SetupPermRepo([perm]);

        await _handler.Handle(command, CancellationToken.None);

        _permRepoMock.Verify(
            r =>
                r.GetByIdsAsync(
                    It.Is<IEnumerable<Guid>>(ids => ids.SequenceEqual(new[] { perm.Id })),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenOwnerRole_WhenUpdatingWithPermissions_ThenPermissionsShouldNotBeReassigned()
    {
        var role = new OrganizationMemberRole(
            _organizationId,
            "Владелец",
            isSystem: true,
            isOwner: true
        );
        var existingPerm = CreatePermission("View");
        role.AddPermission(existingPerm);

        var newPerm = CreatePermission("Delete");
        var command = new UpdateRoleCommand(role.Id, role.Name, role.Description, [newPerm.Id]);

        SetupRepo(role);

        await _handler.Handle(command, CancellationToken.None);

        // AssignPermissions should not be called for owner role
        _permRepoMock.Verify(
            r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        role.Permissions.ShouldHaveSingleItem();
        role.Permissions[0].ShouldBe(existingPerm);
    }

    private void SetupRepo(OrganizationMemberRole role)
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private void SetupPermRepo(List<Permission> permissions)
    {
        _permRepoMock
            .Setup(r =>
                r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(permissions);
    }

    private static OrganizationMemberRole CreateRole(Guid orgId) =>
        new(orgId, "Менеджер", "Управление проектами");

    private static Permission CreatePermission(string code) =>
        new("Organization", code, $"Отображаемое название {code}");
}
