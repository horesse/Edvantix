using Edvantix.Chassis.Caching;
using Edvantix.Organizations.Features.Roles.AssignPermissions;

namespace Edvantix.Organizations.UnitTests.Features;

/// <summary>Unit tests for <see cref="AssignPermissionsCommandHandler"/>.</summary>
public sealed class AssignPermissionsCommandHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
    private readonly Mock<IUserRoleAssignmentRepository> _assignmentRepositoryMock;
    private readonly Mock<IHybridCache> _cacheMock;
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

        _assignmentRepositoryMock = new Mock<IUserRoleAssignmentRepository>();
        _assignmentRepositoryMock
            .Setup(r => r.GetAllByRoleIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _cacheMock = new Mock<IHybridCache>();
        _cacheMock
            .Setup(c => c.RemoveByTagAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        _handler = new AssignPermissionsCommandHandler(
            _roleRepositoryMock.Object,
            _permissionRepositoryMock.Object,
            _assignmentRepositoryMock.Object,
            _cacheMock.Object
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
        var permissions = new List<Permission>([
            CreatePermission("scheduling:read", permId1),
            CreatePermission("scheduling:write", permId2),
        ]);

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _permissionRepositoryMock
            .Setup(p =>
                p.GetByNamesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(permissions);

        var command = new AssignPermissionsCommand
        {
            RoleId = roleId,
            PermissionNames = ["scheduling:read", "scheduling:write"],
        };

        await _handler.Handle(command, CancellationToken.None);

        role.Permissions.Count.ShouldBe(2);
        role.Permissions.Select(p => p.PermissionId)
            .ShouldBe([permId1, permId2], ignoreOrder: true);
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
            .Setup(p =>
                p.GetByNamesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())
            )
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

    [Test]
    public async Task GivenPermissionsAssigned_WhenHandled_ThenCacheInvalidatedForAllAffectedUsers()
    {
        var roleId = Guid.CreateVersion7();
        var role = new Role("Admin", _schoolId);
        role.Id = roleId;

        var profileId1 = Guid.CreateVersion7();
        var profileId2 = Guid.CreateVersion7();

        var assignments = new List<UserRoleAssignment>
        {
            CreateAssignment(profileId1, _schoolId, roleId),
            CreateAssignment(profileId2, _schoolId, roleId),
        };

        var permId = Guid.CreateVersion7();
        var permissions = new List<Permission>([CreatePermission("scheduling:read", permId)]);

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _permissionRepositoryMock
            .Setup(p =>
                p.GetByNamesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(permissions);
        _assignmentRepositoryMock
            .Setup(r => r.GetAllByRoleIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignments);

        var command = new AssignPermissionsCommand
        {
            RoleId = roleId,
            PermissionNames = ["scheduling:read"],
        };

        await _handler.Handle(command, CancellationToken.None);

        // Both affected users should have their cache invalidated
        _cacheMock.Verify(
            c => c.RemoveByTagAsync($"user:{profileId1}:{_schoolId}", It.IsAny<CancellationToken>()),
            Times.Once
        );
        _cacheMock.Verify(
            c => c.RemoveByTagAsync($"user:{profileId2}:{_schoolId}", It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    /// <summary>Creates a <see cref="Permission"/> instance with a preset Id for test usage.</summary>
    private static Permission CreatePermission(string name, Guid id)
    {
        var permission = new Permission(name);
        permission.Id = id;
        return permission;
    }

    /// <summary>Creates a <see cref="UserRoleAssignment"/> instance for test usage.</summary>
    private static UserRoleAssignment CreateAssignment(Guid profileId, Guid schoolId, Guid roleId)
    {
        var assignment = new UserRoleAssignment(profileId, schoolId, roleId);
        assignment.Id = Guid.CreateVersion7();
        assignment.ClearDomainEvents();
        return assignment;
    }
}
