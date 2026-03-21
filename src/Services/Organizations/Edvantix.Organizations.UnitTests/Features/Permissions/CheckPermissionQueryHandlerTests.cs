using Edvantix.Chassis.Caching;
using Edvantix.Organizations.Features.Permissions.CheckPermission;

namespace Edvantix.Organizations.UnitTests.Features.Permissions;

/// <summary>Unit tests for <see cref="GetUserPermissionGrantQueryHandler"/>.</summary>
public sealed class CheckPermissionQueryHandlerTests
{
    private readonly Mock<IHybridCache> _cacheMock;
    private readonly Mock<IUserRoleAssignmentRepository> _assignmentRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
    private readonly GetUserPermissionGrantQueryHandler _handler;

    private readonly Guid _userId = Guid.CreateVersion7();
    private readonly Guid _schoolId = Guid.CreateVersion7();

    public CheckPermissionQueryHandlerTests()
    {
        _cacheMock = new Mock<IHybridCache>();
        _assignmentRepositoryMock = new Mock<IUserRoleAssignmentRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _permissionRepositoryMock = new Mock<IPermissionRepository>();

        _handler = new GetUserPermissionGrantQueryHandler(
            _cacheMock.Object,
            _assignmentRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _permissionRepositoryMock.Object
        );
    }

    [Test]
    public async Task GivenUserHasPermissionViaAssignedRole_WhenCheckPermission_ThenReturnsTrue()
    {
        const string permission = "scheduling:create-slot";
        var roleId = Guid.CreateVersion7();
        var permissionId = Guid.CreateVersion7();

        var assignment = new UserRoleAssignment(_userId, _schoolId, roleId);
        var role = CreateRole(roleId, "Teacher", _schoolId, permissionId);
        var permEntity = CreatePermission(permissionId, permission);

        _assignmentRepositoryMock
            .Setup(r =>
                r.GetByProfileAndSchoolAsync(_userId, _schoolId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([assignment]);

        _roleRepositoryMock
            .Setup(r => r.GetBySchoolAsync(_schoolId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([role]);

        _permissionRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([permEntity]);

        // Cache passthrough — invokes factory directly
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<bool>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns<
                string,
                Func<CancellationToken, ValueTask<bool>>,
                IEnumerable<string>?,
                CancellationToken
            >((_, factory, _, ct) => factory(ct));

        var query = new GetUserPermissionGrantQuery(_userId, _schoolId, permission);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.ShouldBeTrue();
    }

    [Test]
    public async Task GivenUserLacksPermission_WhenCheckPermission_ThenReturnsFalse()
    {
        const string permission = "scheduling:create-slot";
        const string assignedPermission = "scheduling:read";
        var roleId = Guid.CreateVersion7();
        var permissionId = Guid.CreateVersion7();

        var assignment = new UserRoleAssignment(_userId, _schoolId, roleId);
        var role = CreateRole(roleId, "Viewer", _schoolId, permissionId);
        var permEntity = CreatePermission(permissionId, assignedPermission);

        _assignmentRepositoryMock
            .Setup(r =>
                r.GetByProfileAndSchoolAsync(_userId, _schoolId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([assignment]);

        _roleRepositoryMock
            .Setup(r => r.GetBySchoolAsync(_schoolId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([role]);

        _permissionRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([permEntity]);

        // Cache passthrough — invokes factory directly
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<bool>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns<
                string,
                Func<CancellationToken, ValueTask<bool>>,
                IEnumerable<string>?,
                CancellationToken
            >((_, factory, _, ct) => factory(ct));

        var query = new GetUserPermissionGrantQuery(_userId, _schoolId, permission);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.ShouldBeFalse();
    }

    [Test]
    public async Task GivenCachedPermission_WhenCheckedAgain_ThenCacheFactoryNotCalledTwice()
    {
        const string permission = "scheduling:create-slot";
        var expectedKey = $"perm:{_userId}:{_schoolId}:{permission}";
        var expectedTag = $"user:{_userId}:{_schoolId}";

        // Cache returns cached value directly without invoking factory
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync(
                    expectedKey,
                    It.IsAny<Func<CancellationToken, ValueTask<bool>>>(),
                    It.Is<IEnumerable<string>?>(tags => tags != null && tags.Contains(expectedTag)),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        var query = new GetUserPermissionGrantQuery(_userId, _schoolId, permission);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.ShouldBeTrue();

        // Verify cache was called with the correct key format and tag
        _cacheMock.Verify(
            c =>
                c.GetOrCreateAsync(
                    expectedKey,
                    It.IsAny<Func<CancellationToken, ValueTask<bool>>>(),
                    It.Is<IEnumerable<string>?>(tags => tags != null && tags.Contains(expectedTag)),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenUserWithNoAssignments_WhenCheckPermission_ThenReturnsFalse()
    {
        const string permission = "scheduling:create-slot";

        _assignmentRepositoryMock
            .Setup(r =>
                r.GetByProfileAndSchoolAsync(_userId, _schoolId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);

        // Cache passthrough
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<bool>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns<
                string,
                Func<CancellationToken, ValueTask<bool>>,
                IEnumerable<string>?,
                CancellationToken
            >((_, factory, _, ct) => factory(ct));

        var query = new GetUserPermissionGrantQuery(_userId, _schoolId, permission);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.ShouldBeFalse();
    }

    /// <summary>Creates a <see cref="Role"/> for the given school with one permission assigned.</summary>
    private static Role CreateRole(Guid id, string name, Guid schoolId, Guid permissionId)
    {
        var role = new Role(name, schoolId);
        role.Id = id;
        role.AssignPermission(permissionId);
        return role;
    }

    /// <summary>Creates a <see cref="Permission"/> entity with a preset Id.</summary>
    private static Permission CreatePermission(Guid id, string name)
    {
        var permission = new Permission(name);
        permission.Id = id;
        return permission;
    }
}
