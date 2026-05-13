namespace Edvantix.Organizational.UnitTests.Features.Rooms.List;

public sealed class GetAvailableRoomsEndpointTests
{
    private readonly GetAvailableRoomsEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenRequest_WhenHandling_ThenShouldReturnOk()
    {
        var query = new GetAvailableRoomsQuery();
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<RoomDto>)[]);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<IReadOnlyList<RoomDto>>>();
    }

    [Test]
    public async Task GivenRequest_WhenHandling_ThenShouldCallSenderOnce()
    {
        var query = new GetAvailableRoomsQuery(MinCapacity: 20);
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<RoomDto>)[]);

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenRoomsExist_WhenHandling_ThenShouldReturnAllRooms()
    {
        var orgId = Guid.CreateVersion7();
        var rooms = new List<RoomDto>
        {
            new(Guid.CreateVersion7(), orgId, "Каб. 1", Floor: 1, Seats: 10),
            new(Guid.CreateVersion7(), orgId, "Каб. 2", Floor: 2, Seats: 30),
        };
        var query = new GetAvailableRoomsQuery();
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<RoomDto>)rooms);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.Value!.Count.ShouldBe(2);
    }
}
