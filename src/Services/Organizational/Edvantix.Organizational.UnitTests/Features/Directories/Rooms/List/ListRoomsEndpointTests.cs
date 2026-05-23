namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.List;

public sealed class ListRoomsEndpointTests
{
    private readonly ListRoomsEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldCallSenderOnce()
    {
        var query = new ListRoomsQuery();
        var pagedResult = new PagedResult<RoomListItemDto>([], 1, 50, 0);
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldReturnOkWithPagedResult()
    {
        var query = new ListRoomsQuery();
        var pagedResult = new PagedResult<RoomListItemDto>([], 1, 50, 0);
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<PagedResult<RoomListItemDto>>>();
        result.Value.ShouldBe(pagedResult);
    }
}
