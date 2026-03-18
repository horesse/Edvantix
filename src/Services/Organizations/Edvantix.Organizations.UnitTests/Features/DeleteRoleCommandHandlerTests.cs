using Edvantix.Organizations.Features.Roles.DeleteRole;

namespace Edvantix.Organizations.UnitTests.Features;

/// <summary>Unit tests for <see cref="DeleteRoleCommandHandler"/>.</summary>
public sealed class DeleteRoleCommandHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRoleAssignmentRepository> _assignmentRepositoryMock;
    private readonly DeleteRoleCommandHandler _handler;
    private readonly Guid _schoolId = Guid.CreateVersion7();

    public DeleteRoleCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roleRepositoryMock = new Mock<IRoleRepository>();
        _roleRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

        _assignmentRepositoryMock = new Mock<IUserRoleAssignmentRepository>();

        _handler = new DeleteRoleCommandHandler(
            _roleRepositoryMock.Object,
            _assignmentRepositoryMock.Object
        );
    }

    [Test]
    public async Task GivenRoleWithNoAssignments_WhenDeleting_ThenRoleSoftDeleted()
    {
        var roleId = Guid.CreateVersion7();
        var role = new Role("Teacher", _schoolId);

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _assignmentRepositoryMock
            .Setup(a => a.ExistsByRoleIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new DeleteRoleCommand { Id = roleId };
        await _handler.Handle(command, CancellationToken.None);

        role.IsDeleted.ShouldBeTrue();
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenRoleWithAssignments_WhenDeleting_ThenThrowsInvalidOperationException()
    {
        var roleId = Guid.CreateVersion7();
        var role = new Role("Admin", _schoolId);

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _assignmentRepositoryMock
            .Setup(a => a.ExistsByRoleIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DeleteRoleCommand { Id = roleId };
        var act = () => _handler.Handle(command, CancellationToken.None).AsTask();

        await act.ShouldThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task GivenNonExistentRole_WhenDeleting_ThenThrowsNotFoundException()
    {
        var roleId = Guid.CreateVersion7();

        _roleRepositoryMock
            .Setup(r => r.FindByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        var command = new DeleteRoleCommand { Id = roleId };
        var act = () => _handler.Handle(command, CancellationToken.None).AsTask();

        await act.ShouldThrowAsync<NotFoundException>();
    }
}
