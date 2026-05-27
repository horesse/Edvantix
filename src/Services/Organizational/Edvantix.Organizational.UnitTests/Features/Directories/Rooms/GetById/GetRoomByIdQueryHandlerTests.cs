using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Grpc.Services.Groups;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.GetById;

public sealed class GetRoomByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Mock<IMapper<Room, RoomDto>> _mapperMock = new();
    private readonly Mock<IGroupsUsageService> _usageMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly GetRoomByIdQueryHandler _handler;

    public GetRoomByIdQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _usageMock
            .Setup(s =>
                s.CountByRoomIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, int>());
        _handler = new(
            _tenantMock.Object,
            _repoMock.Object,
            _mapperMock.Object,
            _usageMock.Object
        );
    }

    [Test]
    public async Task GivenExistingRoomWithNoGroups_WhenGettingById_ThenShouldReturnDtoWithZeroUsage()
    {
        var room = CreateRoom(_orgId);
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _mapperMock.Setup(m => m.Map(room)).Returns(CreateDto(room));

        var result = await _handler.Handle(new GetRoomByIdQuery(room.Id), CancellationToken.None);

        result.Id.ShouldBe(room.Id);
        result.Usage.ShouldHaveSingleItem();
        result.Usage[0].Label.ShouldBe("Группы");
        result.Usage[0].Count.ShouldBe(0);
    }

    [Test]
    public async Task GivenExistingRoomWithGroups_WhenGettingById_ThenShouldReturnDtoWithGroupCount()
    {
        var room = CreateRoom(_orgId);
        _usageMock
            .Setup(s =>
                s.CountByRoomIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, int> { [room.Id] = 3 });
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _mapperMock.Setup(m => m.Map(room)).Returns(CreateDto(room));

        var result = await _handler.Handle(new GetRoomByIdQuery(room.Id), CancellationToken.None);

        result.Usage.ShouldHaveSingleItem();
        result.Usage[0].Count.ShouldBe(3);
    }

    [Test]
    public async Task GivenRoomNotFound_WhenGettingById_ThenShouldThrowNotFoundException()
    {
        var roomId = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetRoomByIdQuery(roomId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenRoomFromDifferentOrganization_WhenGettingById_ThenShouldThrowNotFoundException()
    {
        var room = CreateRoom(Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetRoomByIdQuery(room.Id), CancellationToken.None).AsTask()
        );
    }

    private static Room CreateRoom(Guid orgId) =>
        new(orgId, "Каб. 101", capacity: 20, floor: "1", RoomType.Classroom);

    private static RoomDto CreateDto(Room room) =>
        new(
            room.Id,
            room.Name,
            room.Capacity,
            room.Floor,
            room.RoomType,
            room.IsArchived,
            room.Order,
            room.OrganizationId,
            room.CreatedAt,
            room.LastModifiedAt,
            room.CreatedBy,
            room.LastModifiedBy,
            Usage: []
        );
}
