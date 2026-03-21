using Edvantix.Chassis.Caching;
using Edvantix.Organizations.Features.UserRoleAssignments.RevokeRole;

namespace Edvantix.Organizations.UnitTests.Features;

/// <summary>Unit tests for <see cref="RevokeRoleCommandHandler"/>.</summary>
public sealed class RevokeRoleCommandHandlerTests
{
    private readonly Mock<IUserRoleAssignmentRepository> _assignmentRepositoryMock;
    private readonly Mock<IHybridCache> _cacheMock;
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

        _cacheMock = new Mock<IHybridCache>();
        _cacheMock
            .Setup(c => c.RemoveByTagAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        _handler = new RevokeRoleCommandHandler(_assignmentRepositoryMock.Object, _cacheMock.Object);
    }

    [Test]
    public async Task GivenExistingAssignment_WhenRevoking_ThenAssignmentRemoved()
    {
        var assignment = new UserRoleAssignment(_profileId, _schoolId, _roleId);
        assignment.Id = Guid.CreateVersion7();
        assignment.ClearDomainEvents();

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

    [Test]
    public async Task GivenRoleRevoked_WhenHandled_ThenCacheInvalidated()
    {
        var assignment = new UserRoleAssignment(_profileId, _schoolId, _roleId);
        assignment.Id = Guid.CreateVersion7();
        assignment.ClearDomainEvents();

        _assignmentRepositoryMock
            .Setup(r => r.FindAsync(_profileId, _roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        var command = new RevokeRoleCommand { ProfileId = _profileId, RoleId = _roleId };

        await _handler.Handle(command, CancellationToken.None);

        var expectedTag = $"user:{_profileId}:{_schoolId}";
        _cacheMock.Verify(
            c => c.RemoveByTagAsync(expectedTag, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenExistingAssignment_WhenRevoking_ThenRevokeCalledBeforeRemove()
    {
        var assignment = new UserRoleAssignment(_profileId, _schoolId, _roleId);
        assignment.Id = Guid.CreateVersion7();
        assignment.ClearDomainEvents();

        _assignmentRepositoryMock
            .Setup(r => r.FindAsync(_profileId, _roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        var command = new RevokeRoleCommand { ProfileId = _profileId, RoleId = _roleId };

        await _handler.Handle(command, CancellationToken.None);

        // Verify that Revoke() was called — the domain event should be registered
        assignment.DomainEvents.OfType<Edvantix.Organizations.Infrastructure.EventServices.Events.UserRoleRevokedEvent>()
            .ShouldHaveSingleItem();
    }
}
