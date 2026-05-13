namespace Edvantix.Organizational.UnitTests.Features.Rooms.List;

public sealed class GetAvailableRoomsQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Mock<IMapper<Room, RoomDto>> _mapperMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetAvailableRoomsQueryHandler _handler;

    public GetAvailableRoomsQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenRoomsExist_WhenNoMinCapacity_ThenShouldReturnAllRoomsSortedBySeats()
    {
        var room30 = CreateRoom(_organizationId, "Зал А", seats: 30);
        var room10 = CreateRoom(_organizationId, "Каб. 1", seats: 10);
        var room20 = CreateRoom(_organizationId, "Каб. 2", seats: 20);

        _repoMock
            .Setup(r => r.ListByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([room30, room10, room20]);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<Room>()))
            .Returns<Room>(r => new RoomDto(r.Id, r.OrganizationId, r.Label, r.Floor, r.Seats));

        var result = await _handler.Handle(new GetAvailableRoomsQuery(), CancellationToken.None);

        result.Count.ShouldBe(3);
        result[0].Seats.ShouldBe((short)10);
        result[1].Seats.ShouldBe((short)20);
        result[2].Seats.ShouldBe((short)30);
    }

    [Test]
    public async Task GivenRoomsExist_WhenMinCapacitySet_ThenFittingRoomsComeFirst()
    {
        var room10 = CreateRoom(_organizationId, "Маленький", seats: 10);
        var room30 = CreateRoom(_organizationId, "Большой", seats: 30);

        _repoMock
            .Setup(r => r.ListByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([room10, room30]);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<Room>()))
            .Returns<Room>(r => new RoomDto(r.Id, r.OrganizationId, r.Label, r.Floor, r.Seats));

        var result = await _handler.Handle(
            new GetAvailableRoomsQuery(MinCapacity: 20),
            CancellationToken.None
        );

        result.Count.ShouldBe(2);
        result[0].Seats.ShouldBe((short)30);
        result[1].Seats.ShouldBe((short)10);
    }

    [Test]
    public async Task GivenRoomWithMinimalHeadroom_WhenMinCapacitySet_ThenFitsTightIsTrue()
    {
        var roomTight = CreateRoom(_organizationId, "Тесный", seats: 20);

        _repoMock
            .Setup(r => r.ListByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([roomTight]);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<Room>()))
            .Returns<Room>(r => new RoomDto(r.Id, r.OrganizationId, r.Label, r.Floor, r.Seats));

        // 20 мест и требуется 18 → запас менее 30%, должен быть fitsTight
        var result = await _handler.Handle(
            new GetAvailableRoomsQuery(MinCapacity: 18),
            CancellationToken.None
        );

        result[0].FitsTight.ShouldBeTrue();
    }

    [Test]
    public async Task GivenRoomWithAmpleHeadroom_WhenMinCapacitySet_ThenFitsTightIsFalse()
    {
        var roomAmple = CreateRoom(_organizationId, "Просторный", seats: 50);

        _repoMock
            .Setup(r => r.ListByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([roomAmple]);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<Room>()))
            .Returns<Room>(r => new RoomDto(r.Id, r.OrganizationId, r.Label, r.Floor, r.Seats));

        // 50 мест и требуется 10 → запас более 30%, не тесный
        var result = await _handler.Handle(
            new GetAvailableRoomsQuery(MinCapacity: 10),
            CancellationToken.None
        );

        result[0].FitsTight.ShouldBeFalse();
    }

    [Test]
    public async Task GivenRoomTooSmall_WhenMinCapacitySet_ThenFitsTightIsFalse()
    {
        var roomSmall = CreateRoom(_organizationId, "Слишком маленький", seats: 5);

        _repoMock
            .Setup(r => r.ListByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([roomSmall]);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<Room>()))
            .Returns<Room>(r => new RoomDto(r.Id, r.OrganizationId, r.Label, r.Floor, r.Seats));

        var result = await _handler.Handle(
            new GetAvailableRoomsQuery(MinCapacity: 20),
            CancellationToken.None
        );

        result[0].FitsTight.ShouldBeFalse();
    }

    [Test]
    public async Task GivenNoRooms_WhenHandling_ThenShouldReturnEmptyList()
    {
        _repoMock
            .Setup(r => r.ListByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(new GetAvailableRoomsQuery(), CancellationToken.None);

        result.ShouldBeEmpty();
    }

    private static Room CreateRoom(Guid orgId, string label, short seats) =>
        new(orgId, label, floor: 1, seats: seats);
}
