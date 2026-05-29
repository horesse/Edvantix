using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.Archive;

public sealed class ArchiveRoomCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly ArchiveRoomCommandHandler _handler;

    public ArchiveRoomCommandHandlerTests()
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
    public async Task GivenActiveRoom_WhenArchiving_ThenShouldSetIsArchivedAndSave()
    {
        var room = CreateRoom(_orgId);
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        await _handler.Handle(new ArchiveRoomCommand(room.Id), CancellationToken.None);

        room.IsDeleted.ShouldBeTrue();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenAlreadyArchivedRoom_WhenArchiving_ThenShouldBeIdempotent()
    {
        var room = CreateRoom(_orgId);
        room.Archive(_profileId);
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        await _handler.Handle(new ArchiveRoomCommand(room.Id), CancellationToken.None);

        room.IsDeleted.ShouldBeTrue();
    }

    [Test]
    public async Task GivenRoomNotFound_WhenArchiving_ThenShouldThrowNotFoundException()
    {
        var roomId = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new ArchiveRoomCommand(roomId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenRoomFromDifferentOrganization_WhenArchiving_ThenShouldThrowNotFoundException()
    {
        var room = CreateRoom(Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new ArchiveRoomCommand(room.Id), CancellationToken.None).AsTask()
        );
    }

    private static Room CreateRoom(Guid orgId) =>
        new(orgId, "Каб. 101", capacity: 20, floor: "1", RoomType.Classroom);
}
