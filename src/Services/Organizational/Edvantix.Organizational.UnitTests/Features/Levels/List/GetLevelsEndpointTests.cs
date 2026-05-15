namespace Edvantix.Organizational.UnitTests.Features.Levels.List;

public sealed class GetLevelsEndpointTests
{
    private readonly GetLevelsEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenRequest_WhenHandling_ThenCallsSenderOnce()
    {
        var query = new GetLevelsQuery();

        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LevelDto>());

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenLevelsReturned_WhenHandling_ThenReturnsOkWithList()
    {
        var query = new GetLevelsQuery();
        var levels = new List<LevelDto>
        {
            new(Guid.CreateVersion7(), "A1", "Beginner", null, LevelTone.Blue, 1, true, 0),
            new(Guid.CreateVersion7(), "B1", "Intermediate", null, LevelTone.Teal, 2, true, 0),
        };

        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(levels);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<IReadOnlyList<LevelDto>>>();
        result.Value!.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenEmptyResult_WhenHandling_ThenReturnsOkWithEmptyList()
    {
        var query = new GetLevelsQuery();

        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LevelDto>());

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<IReadOnlyList<LevelDto>>>();
        result.Value.ShouldBeEmpty();
    }
}
