using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.Create;

public sealed class CreateRoomEndpointTests
{
    private readonly CreateRoomEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
    {
        var command = new CreateRoomCommand("Каб. 204", 30, "2", RoomType.Classroom);
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDto());

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnCreated()
    {
        var command = new CreateRoomCommand("Зал А", 50, null, RoomType.Meeting);
        var dto = CreateDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<Created<RoomDto>>();
        result.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenLocationShouldContainId()
    {
        var command = new CreateRoomCommand("Каб. 101", 20, "1", RoomType.Classroom);
        var dto = CreateDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.Location.ShouldNotBeNull();
        result.Location!.ShouldContain(dto.Id.ToString());
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
            null,
            Usage: []
        );
}
