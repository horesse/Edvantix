namespace Edvantix.Groups.UnitTests.Features.Groups.List;

public sealed class GetGroupsEndpointTests
{
    private readonly GetGroupsEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenRequest_WhenHandling_ThenShouldCallSenderOnce()
    {
        var query = new GetGroupsQuery();
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<GroupListItemDto>([], 1, 10, 0));

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenRequest_WhenHandling_ThenShouldReturnOkWithPagedResult()
    {
        var query = new GetGroupsQuery();
        var pagedResult = new PagedResult<GroupListItemDto>([], 1, 10, 0);
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<PagedResult<GroupListItemDto>>>();
        result.Value.ShouldBe(pagedResult);
    }
}
