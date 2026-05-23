using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.GetById;

public sealed class GetRoomByIdEndpointTests
{
    private readonly GetRoomByIdEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenExistingRoom_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var id = Guid.CreateVersion7();
        var dto = CreateDto(id);
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetRoomByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        var okResult = result.Result.ShouldBeOfType<Ok<RoomDto>>();
        okResult.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenRoomNotFound_WhenHandling_ThenShouldReturnNotFound()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetRoomByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(NotFoundException.For<Room>(id));

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.Result.ShouldBeOfType<NotFound>();
    }

    private static RoomDto CreateDto(Guid id) =>
        new(
            id,
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
