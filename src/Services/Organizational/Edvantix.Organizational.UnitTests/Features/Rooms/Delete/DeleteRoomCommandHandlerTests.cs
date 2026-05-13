namespace Edvantix.Organizational.UnitTests.Features.Rooms.Delete;

public sealed class DeleteRoomCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly DeleteRoomCommandHandler _handler;

    public DeleteRoomCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingRoom_WhenDeleting_ThenShouldSoftDeleteAndSave()
    {
        var room = CreateRoom(_organizationId);
        var command = new DeleteRoomCommand(room.Id);

        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        room.IsDeleted.ShouldBeTrue();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenRoomNotFound_WhenDeleting_ThenShouldThrowNotFoundException()
    {
        var roomId = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteRoomCommand(roomId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenRoomFromDifferentOrganization_WhenDeleting_ThenShouldThrowNotFoundException()
    {
        var room = CreateRoom(Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteRoomCommand(room.Id), CancellationToken.None).AsTask()
        );
    }

    private static Room CreateRoom(Guid orgId) => new(orgId, "Каб. 204", floor: 2, seats: 20);
}
