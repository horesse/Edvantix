using Edvantix.Organizations.Features.UserRoleAssignments.RevokeRole;

namespace Edvantix.Organizations.UnitTests.Features;

/// <summary>Unit tests for <see cref="RevokeRoleCommandHandler"/>.</summary>
public sealed class RevokeRoleCommandHandlerTests
{
    private readonly Mock<IUserRoleAssignmentRepository> _assignmentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RevokeRoleCommandHandler _handler;

    private readonly Guid _schoolId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly Guid _roleId = Guid.CreateVersion7();

    public RevokeRoleCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _assignmentRepositoryMock = new Mock<IUserRoleAssignmentRepository>();
        _assignmentRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

        _handler = new RevokeRoleCommandHandler(_assignmentRepositoryMock.Object);
    }

    [Test]
    public async Task GivenExistingAssignment_WhenRevoking_ThenAssignmentRemoved()
    {
        var assignment = new UserRoleAssignment(_profileId, _schoolId, _roleId);
        assignment.Id = Guid.CreateVersion7();

        _assignmentRepositoryMock
            .Setup(r => r.FindAsync(_profileId, _roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        var command = new RevokeRoleCommand { ProfileId = _profileId, RoleId = _roleId };

        await _handler.Handle(command, CancellationToken.None);

        _assignmentRepositoryMock.Verify(r => r.Remove(assignment), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenNonExistentAssignment_WhenRevoking_ThenThrowsNotFoundException()
    {
        _assignmentRepositoryMock
            .Setup(r => r.FindAsync(_profileId, _roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserRoleAssignment?)null);

        var command = new RevokeRoleCommand { ProfileId = _profileId, RoleId = _roleId };

        var act = () => _handler.Handle(command, CancellationToken.None).AsTask();

        await act.ShouldThrowAsync<NotFoundException>();
    }
}
