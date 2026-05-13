namespace Edvantix.Organizational.UnitTests.Features.Groups.Stats;

public sealed class GetGroupStatsEndpointTests
{
    private readonly GetGroupStatsEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenRequest_WhenHandling_ThenShouldCallSenderOnce()
    {
        var query = new GetGroupStatsQuery();
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetGroupStatsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GroupStatsDto(0, 0, 0, 0, 0, 0));

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<GetGroupStatsQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenRequest_WhenHandling_ThenShouldReturnOkWithStats()
    {
        var query = new GetGroupStatsQuery();
        var stats = new GroupStatsDto(10, 3, 4, 1, 1, 1);
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetGroupStatsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stats);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<GroupStatsDto>>();
        result.Value.ShouldBe(stats);
    }
}
