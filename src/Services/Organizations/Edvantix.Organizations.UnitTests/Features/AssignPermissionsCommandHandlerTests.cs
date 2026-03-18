using Edvantix.Organizations.Features.Roles.AssignPermissions;

namespace Edvantix.Organizations.UnitTests.Features;

/// <summary>Unit tests for <see cref="AssignPermissionsCommandHandler"/>.</summary>
public sealed class AssignPermissionsCommandHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
    private readonly AssignPermissionsCommandHandler _handler;
    private readonly Guid _schoolId = Guid.CreateVersion7();

    public AssignPermissionsCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roleRepositoryMock = new Mock<IRoleRepository>();
        _roleRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

        _permissionRepositoryMock = new Mock<IPermissionRepository>();

        _handler = new AssignPermissionsCommandHandler(
            _roleRepositoryMock.Object,
            _permissionRepositoryMock.Object
        );
    }

    [Test]
    public async Task GivenValidPermissionNames_WhenAssigning_ThenRoleSetPermissionsCalled()
    {
        var roleId = Guid.CreateVersion7();
        var role = new Role("Admin", _schoolId);
        role.Id = Guid.CreateVersion7();

        var permId1 = Guid.CreateVersion7();
        var permId2 = Guid.CreateVersion7();
        var permissions = new List<Permission>(
        [
            CreatePermission("scheduling:read", permId1),
            CreatePermission("scheduling:write", permId2),
        ]);

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _permissionRepositoryMock
            .Setup(p => p.GetByNamesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(permissions);

        var command = new AssignPermissionsCommand
        {
            RoleId = roleId,
            PermissionNames = ["scheduling:read", "scheduling:write"],
        };

        await _handler.Handle(command, CancellationToken.None);

        role.Permissions.Count.ShouldBe(2);
        role.Permissions.Select(p => p.PermissionId).ShouldBe([permId1, permId2], ignoreOrder: true);
    }

    [Test]
    public async Task GivenSomeInvalidPermissionNames_WhenAssigning_ThenThrowsInvalidOperationException()
    {
        var roleId = Guid.CreateVersion7();
        var role = new Role("Admin", _schoolId);
        role.Id = Guid.CreateVersion7();

        // Only one of the two requested permissions exists in the catalogue
        var permId = Guid.CreateVersion7();
        var permissions = new List<Permission>([CreatePermission("scheduling:read", permId)]);

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _permissionRepositoryMock
            .Setup(p => p.GetByNamesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(permissions);

        var command = new AssignPermissionsCommand
        {
            RoleId = roleId,
            PermissionNames = ["scheduling:read", "unknown:permission"],
        };

        var act = () => _handler.Handle(command, CancellationToken.None).AsTask();

        var ex = await act.ShouldThrowAsync<InvalidOperationException>();
        ex.Message.ShouldContain("unknown:permission");
    }

    [Test]
    public async Task GivenNonExistentRole_WhenAssigning_ThenThrowsNotFoundException()
    {
        var roleId = Guid.CreateVersion7();

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        var command = new AssignPermissionsCommand
        {
            RoleId = roleId,
            PermissionNames = ["scheduling:read"],
        };

        var act = () => _handler.Handle(command, CancellationToken.None).AsTask();

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenEmptyPermissionList_WhenAssigning_ThenRolePermissionsCleared()
    {
        var roleId = Guid.CreateVersion7();
        var role = new Role("Admin", _schoolId);
        role.Id = Guid.CreateVersion7();
        role.AssignPermission(Guid.CreateVersion7());

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var command = new AssignPermissionsCommand { RoleId = roleId, PermissionNames = [] };

        await _handler.Handle(command, CancellationToken.None);

        role.Permissions.ShouldBeEmpty();
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>Creates a <see cref="Permission"/> instance with a preset Id for test usage.</summary>
    private static Permission CreatePermission(string name, Guid id)
    {
        var permission = new Permission(name);
        permission.Id = id;
        return permission;
    }
}
