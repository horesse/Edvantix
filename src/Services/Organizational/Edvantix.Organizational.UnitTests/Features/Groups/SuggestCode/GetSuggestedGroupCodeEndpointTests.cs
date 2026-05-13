namespace Edvantix.Organizational.UnitTests.Features.Groups.SuggestCode;

public sealed class GetSuggestedGroupCodeEndpointTests
{
    private readonly SuggestGroupCodeEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidQuery_WhenHandling_ThenShouldCallSenderOnce()
    {
        var query = new GetSuggestedGroupCodeQuery(GroupLevel.B1);
        _senderMock.Setup(s => s.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync("B1-01");

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidQuery_WhenHandling_ThenShouldReturnOkWithCode()
    {
        var query = new GetSuggestedGroupCodeQuery(GroupLevel.B1);
        _senderMock.Setup(s => s.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync("B1-01");

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<string>>();
        result.Value.ShouldBe("B1-01");
    }
}
