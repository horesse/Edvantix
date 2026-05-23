using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.List;

public sealed class ListRoomsQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Mock<IMapper<Room, RoomListItemDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly ListRoomsQueryHandler _handler;

    public ListRoomsQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenActiveRooms_WhenListing_ThenShouldReturnPagedResult()
    {
        var rooms = new List<Room>
        {
            new(_orgId, "Каб. 204", 30, "2", RoomType.Classroom),
            new(_orgId, "Зал А", 100, "3", RoomType.Lab),
        };
        SetupList(rooms);
        SetupCount(2);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<Room>>()))
            .Returns(rooms.Select(MapToDto).ToList());

        var result = await _handler.Handle(new ListRoomsQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(2);
        result.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenEmptyOrganization_WhenListing_ThenShouldReturnEmptyResult()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<Room>>()))
            .Returns(Array.Empty<RoomListItemDto>());

        var result = await _handler.Handle(new ListRoomsQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(0);
        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenIncludeArchivedFalse_WhenListing_ThenShouldCallListAndCount()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<Room>>()))
            .Returns(Array.Empty<RoomListItemDto>());

        await _handler.Handle(new ListRoomsQuery(IncludeArchived: false), CancellationToken.None);

        _repoMock.Verify(
            r => r.ListAsync(It.IsAny<ISpecification<Room>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _repoMock.Verify(
            r => r.CountAsync(It.IsAny<ISpecification<Room>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenSearchTerm_WhenListing_ThenShouldCallBothSpecifications()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<Room>>()))
            .Returns(Array.Empty<RoomListItemDto>());

        await _handler.Handle(new ListRoomsQuery(Search: "Каб"), CancellationToken.None);

        _repoMock.Verify(
            r => r.ListAsync(It.IsAny<ISpecification<Room>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private void SetupList(IReadOnlyList<Room> items) =>
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Room>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(items);

    private void SetupCount(int count) =>
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Room>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(count);

    private static RoomListItemDto MapToDto(Room r) =>
        new(r.Id, r.Name, r.Capacity, r.Floor, r.RoomType, r.IsArchived, r.Order);
}
