using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.Restore;

public sealed class RestoreRoomCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly RestoreRoomCommandHandler _handler;

    public RestoreRoomCommandHandlerTests()
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
    public async Task GivenArchivedRoom_WhenRestoring_ThenShouldClearIsArchivedAndSave()
    {
        var room = CreateArchivedRoom(_orgId);
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        await _handler.Handle(new RestoreRoomCommand(room.Id), CancellationToken.None);

        room.IsArchived.ShouldBeFalse();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenActiveRoom_WhenRestoring_ThenShouldBeIdempotent()
    {
        var room = CreateRoom(_orgId);
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        await _handler.Handle(new RestoreRoomCommand(room.Id), CancellationToken.None);

        room.IsArchived.ShouldBeFalse();
    }

    [Test]
    public async Task GivenRoomNotFound_WhenRestoring_ThenShouldThrowNotFoundException()
    {
        var roomId = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new RestoreRoomCommand(roomId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenRoomFromDifferentOrganization_WhenRestoring_ThenShouldThrowNotFoundException()
    {
        var room = CreateRoom(Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new RestoreRoomCommand(room.Id), CancellationToken.None).AsTask()
        );
    }

    private static Room CreateRoom(Guid orgId) =>
        new(orgId, "Каб. 101", capacity: 20, floor: "1", RoomType.Classroom);

    private static Room CreateArchivedRoom(Guid orgId)
    {
        var room = CreateRoom(orgId);
        room.Archive(Guid.CreateVersion7());

        return room;
    }
}
