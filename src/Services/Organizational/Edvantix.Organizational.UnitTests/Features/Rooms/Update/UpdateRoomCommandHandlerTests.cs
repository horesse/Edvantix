namespace Edvantix.Organizational.UnitTests.Features.Rooms.Update;

public sealed class UpdateRoomCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly UpdateRoomCommandHandler _handler;

    public UpdateRoomCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingRoom_WhenUpdating_ThenShouldUpdateAndSave()
    {
        var room = CreateRoom(_organizationId);
        var command = new UpdateRoomCommand(room.Id, "Новый зал", Floor: 4, Seats: 30);

        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        room.Label.ShouldBe("Новый зал");
        room.Floor.ShouldBe((short)4);
        room.Seats.ShouldBe((short)30);
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenRoomNotFound_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var roomId = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new UpdateRoomCommand(roomId, "Зал", Floor: 1, Seats: 20),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenRoomFromDifferentOrganization_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var room = CreateRoom(Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new UpdateRoomCommand(room.Id, "Зал", Floor: 1, Seats: 20),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    private static Room CreateRoom(Guid orgId) => new(orgId, "Каб. 101", floor: 1, seats: 15);
}
