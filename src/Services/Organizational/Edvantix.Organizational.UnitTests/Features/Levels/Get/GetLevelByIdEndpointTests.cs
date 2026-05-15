namespace Edvantix.Organizational.UnitTests.Features.Levels.Get;

public sealed class GetLevelByIdEndpointTests
{
    private readonly GetLevelByIdEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidId_WhenHandling_ThenSendsQueryWithCorrectId()
    {
        var id = Guid.CreateVersion7();
        var dto = CreateDto(id);

        _senderMock
            .Setup(s =>
                s.Send(It.Is<GetLevelByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(dto);

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.Is<GetLevelByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidId_WhenHandling_ThenReturnsOkWithDto()
    {
        var id = Guid.CreateVersion7();
        var dto = CreateDto(id);

        _senderMock
            .Setup(s => s.Send(It.IsAny<GetLevelByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.ShouldBeOfType<Ok<LevelDto>>();
        result.Value.ShouldBe(dto);
    }

    private static LevelDto CreateDto(Guid id) =>
        new(
            id,
            "A1",
            "Beginner",
            null,
            LevelTone.Blue,
            SortOrder: 1,
            IsActive: true,
            UsageCount: 0
        );
}
