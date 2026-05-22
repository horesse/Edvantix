namespace Edvantix.Groups.UnitTests.Features.Directories.Levels.List;

public sealed class ListLevelsDirectoryEndpointTests
{
    private readonly ListLevelsDirectoryEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenQuery_WhenHandling_ThenDelegatesToSender()
    {
        var query = new ListLevelsDirectoryQuery();
        SetupSender(query, BuildPagedResult([]));

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenQuery_WhenHandling_ThenReturnsOkWithPagedResult()
    {
        var query = new ListLevelsDirectoryQuery();
        var paged = BuildPagedResult(
            [new(Guid.CreateVersion7(), "Beginner", 1, null, false)]
        );
        SetupSender(query, paged);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        var ok = result.ShouldBeOfType<Ok<PagedResult<LevelDirectoryListItemDto>>>();
        ok.Value.ShouldBe(paged);
    }

    private void SetupSender(
        ListLevelsDirectoryQuery query,
        PagedResult<LevelDirectoryListItemDto> result
    ) => _senderMock.Setup(s => s.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(result);

    private static PagedResult<LevelDirectoryListItemDto> BuildPagedResult(
        IReadOnlyList<LevelDirectoryListItemDto> items
    ) => new(items, 1, 20, items.Count);
}
