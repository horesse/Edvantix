using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.Create;

public sealed class CreateRoomCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Mock<IMapper<Room, RoomDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly CreateRoomCommandHandler _handler;

    public CreateRoomCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
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
    public async Task GivenValidCommand_WhenHandling_ThenShouldSaveAndReturnDto()
    {
        var expectedDto = CreateDto();
        var command = new CreateRoomCommand("Каб. 204", 30, "2", RoomType.Classroom);
        _mapperMock.Setup(m => m.Map(It.IsAny<Room>())).Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBe(expectedDto);
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenRoomShouldBelongToCurrentOrganization()
    {
        Room? capturedRoom = null;
        var command = new CreateRoomCommand("Лекционный зал", 100, "3", RoomType.Lab, Order: 1);
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
            .Callback<Room, CancellationToken>((room, _) => capturedRoom = room)
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map(It.IsAny<Room>())).Returns(CreateDto());

        await _handler.Handle(command, CancellationToken.None);

        capturedRoom.ShouldNotBeNull();
        capturedRoom!.OrganizationId.ShouldBe(_orgId);
        capturedRoom.Name.ShouldBe("Лекционный зал");
        capturedRoom.Capacity.ShouldBe(100);
        capturedRoom.Floor.ShouldBe("3");
        capturedRoom.RoomType.ShouldBe(RoomType.Lab);
        capturedRoom.IsArchived.ShouldBeFalse();
    }

    private static RoomDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "Каб. 204",
            30,
            "2",
            RoomType.Classroom,
            false,
            0,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
