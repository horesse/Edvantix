namespace Edvantix.Organizational.UnitTests.Features.Rooms.Create;

public sealed class CreateRoomCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly CreateRoomCommandHandler _handler;

    public CreateRoomCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldSaveChanges()
    {
        var command = new CreateRoomCommand("Зал А", Floor: 1, Seats: 50);

        _repoMock
            .Setup(r =>
                r.AddAsync(
                    It.IsAny<Room>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenRoomShouldBelongToCurrentOrganization()
    {
        Room? capturedRoom = null;
        var command = new CreateRoomCommand("Лекционный зал", Floor: 3, Seats: 100);

        _repoMock
            .Setup(r =>
                r.AddAsync(
                    It.IsAny<Room>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback<Room, CancellationToken>(
                (room, _) => capturedRoom = room
            )
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        capturedRoom.ShouldNotBeNull();
        capturedRoom.OrganizationId.ShouldBe(_organizationId);
        capturedRoom.Label.ShouldBe("Лекционный зал");
        capturedRoom.Floor.ShouldBe((short)3);
        capturedRoom.Seats.ShouldBe((short)100);
    }
}
