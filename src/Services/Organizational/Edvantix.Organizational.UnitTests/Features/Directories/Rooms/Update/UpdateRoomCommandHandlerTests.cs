using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.Update;

public sealed class UpdateRoomCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Mock<IMapper<Room, RoomDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly UpdateRoomCommandHandler _handler;

    public UpdateRoomCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(
            _tenantMock.Object,
            _claimsMock.Object,
            _repoMock.Object,
            _mapperMock.Object
        );
    }

    [Test]
    public async Task GivenExistingRoom_WhenUpdating_ThenShouldUpdateAndSave()
    {
        var room = CreateRoom(_orgId);
        var command = new UpdateRoomCommand(room.Id, "Новый зал", 200, "4", RoomType.Lab, 5);
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _mapperMock.Setup(m => m.Map(room)).Returns(CreateDto());

        await _handler.Handle(command, CancellationToken.None);

        room.Name.ShouldBe("Новый зал");
        room.Capacity.ShouldBe(200);
        room.Floor.ShouldBe("4");
        room.RoomType.ShouldBe(RoomType.Lab);
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
                    new UpdateRoomCommand(roomId, "Зал", 30, null, RoomType.Classroom),
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
                    new UpdateRoomCommand(room.Id, "Зал", 30, null, RoomType.Classroom),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    private static Room CreateRoom(Guid orgId) =>
        new(orgId, "Каб. 101", capacity: 20, floor: "1", RoomType.Classroom);

    private static RoomDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "Новый зал",
            200,
            "4",
            RoomType.Lab,
            false,
            5,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null,
            Usage: []
        );
}
