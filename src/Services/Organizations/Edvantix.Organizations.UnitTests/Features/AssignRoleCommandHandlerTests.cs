using Edvantix.Organizations.Features.UserRoleAssignments.AssignRole;
using Edvantix.Organizations.Grpc.Services;

namespace Edvantix.Organizations.UnitTests.Features;

/// <summary>Unit tests for <see cref="AssignRoleCommandHandler"/>.</summary>
public sealed class AssignRoleCommandHandlerTests
{
    private readonly Mock<IUserRoleAssignmentRepository> _assignmentRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IPersonaProfileService> _personaProfileServiceMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AssignRoleCommandHandler _handler;

    private readonly Guid _schoolId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly Guid _roleId = Guid.CreateVersion7();

    public AssignRoleCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _assignmentRepositoryMock = new Mock<IUserRoleAssignmentRepository>();
        _assignmentRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _assignmentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<UserRoleAssignment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (UserRoleAssignment a, CancellationToken _) =>
                {
                    a.Id = Guid.CreateVersion7();
                    return a;
                }
            );

        _roleRepositoryMock = new Mock<IRoleRepository>();
        _personaProfileServiceMock = new Mock<IPersonaProfileService>();

        _tenantContextMock = new Mock<ITenantContext>();
        _tenantContextMock.SetupGet(t => t.SchoolId).Returns(_schoolId);

        _handler = new AssignRoleCommandHandler(
            _assignmentRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _personaProfileServiceMock.Object,
            _tenantContextMock.Object
        );
    }

    [Test]
    public async Task GivenValidProfileIdAndRoleId_WhenAssigning_ThenAssignmentCreated()
    {
        var role = CreateRole(_roleId, "Teacher");

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(_roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _personaProfileServiceMock
            .Setup(s => s.ValidateProfileExistsAsync(_profileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _assignmentRepositoryMock
            .Setup(r => r.FindAsync(_profileId, _roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserRoleAssignment?)null);

        var command = new AssignRoleCommand { ProfileId = _profileId, RoleId = _roleId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBe(Guid.Empty);
        _assignmentRepositoryMock.Verify(
            r =>
                r.AddAsync(
                    It.Is<UserRoleAssignment>(a =>
                        a.ProfileId == _profileId && a.RoleId == _roleId && a.SchoolId == _schoolId
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenInvalidProfileId_WhenAssigning_ThenThrowsNotFoundException()
    {
        var role = CreateRole(_roleId, "Teacher");

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(_roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _personaProfileServiceMock
            .Setup(s => s.ValidateProfileExistsAsync(_profileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new AssignRoleCommand { ProfileId = _profileId, RoleId = _roleId };

        var act = () => _handler.Handle(command, CancellationToken.None).AsTask();

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenNonExistentRole_WhenAssigning_ThenThrowsNotFoundException()
    {
        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(_roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        var command = new AssignRoleCommand { ProfileId = _profileId, RoleId = _roleId };

        var act = () => _handler.Handle(command, CancellationToken.None).AsTask();

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenDuplicateAssignment_WhenAssigning_ThenThrowsInvalidOperationException()
    {
        var role = CreateRole(_roleId, "Teacher");
        var existing = new UserRoleAssignment(_profileId, _schoolId, _roleId);
        existing.Id = Guid.CreateVersion7();

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(_roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _personaProfileServiceMock
            .Setup(s => s.ValidateProfileExistsAsync(_profileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _assignmentRepositoryMock
            .Setup(r => r.FindAsync(_profileId, _roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new AssignRoleCommand { ProfileId = _profileId, RoleId = _roleId };

        var act = () => _handler.Handle(command, CancellationToken.None).AsTask();

        await act.ShouldThrowAsync<InvalidOperationException>();
    }

    /// <summary>Creates a <see cref="Role"/> instance with a preset Id for test usage.</summary>
    private Role CreateRole(Guid id, string name)
    {
        var role = new Role(name, _schoolId);
        role.Id = id;
        return role;
    }
}
