using Edvantix.Organizational.Features.Directories.Rooms.Reorder;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.Reorder;

public sealed class ReorderRoomsCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly ReorderRoomsCommandHandler _handler;

    public ReorderRoomsCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(_tenantMock.Object, _claimsMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenThreeRooms_WhenReordering_ThenOrderShouldMatchIndexPosition()
    {
        var r1 = CreateRoom();
        var r2 = CreateRoom();
        var r3 = CreateRoom();
        SetupList([r1, r2, r3]);

        await _handler.Handle(
            new ReorderRoomsCommand([r3.Id, r1.Id, r2.Id]),
            CancellationToken.None
        );

        r3.Order.ShouldBe(0);
        r1.Order.ShouldBe(1);
        r2.Order.ShouldBe(2);
    }

    [Test]
    public async Task GivenReordering_WhenCalled_ThenShouldSaveOnce()
    {
        var room = CreateRoom();
        SetupList([room]);

        await _handler.Handle(new ReorderRoomsCommand([room.Id]), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenUnknownId_WhenReordering_ThenShouldBeIgnored()
    {
        var room = CreateRoom();
        SetupList([room]);
        var unknownId = Guid.CreateVersion7();

        await _handler.Handle(
            new ReorderRoomsCommand([unknownId, room.Id]),
            CancellationToken.None
        );

        room.Order.ShouldBe(1);
    }

    private Room CreateRoom()
    {
        var room = new Room(_orgId, "Каб. 101", capacity: 20, floor: "1", RoomType.Classroom);
        room.Id = Guid.CreateVersion7();
        return room;
    }

    private void SetupList(IReadOnlyList<Room> items) =>
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Room>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(items);
}
