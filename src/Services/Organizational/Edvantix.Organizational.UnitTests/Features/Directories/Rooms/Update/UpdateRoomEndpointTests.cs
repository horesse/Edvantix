using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.Update;

public sealed class UpdateRoomEndpointTests
{
    private readonly UpdateRoomEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
    {
        var command = new UpdateRoomCommand(
            Guid.CreateVersion7(),
            "Каб. 204",
            30,
            "2",
            RoomType.Classroom
        );
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDto());

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var dto = CreateDto();
        var command = new UpdateRoomCommand(
            Guid.CreateVersion7(),
            "Зал А",
            50,
            null,
            RoomType.Meeting
        );
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<Ok<RoomDto>>();
        result.Value.ShouldBe(dto);
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
