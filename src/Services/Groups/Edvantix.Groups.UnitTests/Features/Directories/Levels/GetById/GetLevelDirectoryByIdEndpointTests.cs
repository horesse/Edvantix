namespace Edvantix.Groups.UnitTests.Features.Directories.Levels.GetById;

public sealed class GetLevelDirectoryByIdEndpointTests
{
    private readonly GetLevelDirectoryByIdEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenExistingLevel_WhenHandling_ThenDelegatesToSender()
    {
        var id = Guid.CreateVersion7();
        SetupSender(id, BuildDto(id));

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<GetLevelDirectoryByIdQuery>(q => q.Id == id),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenExistingLevel_WhenHandling_ThenReturnsOkWithDto()
    {
        var id = Guid.CreateVersion7();
        var dto = BuildDto(id);
        SetupSender(id, dto);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        var ok = result.Result.ShouldBeOfType<Ok<LevelDirectoryDto>>();
        ok.Value.ShouldBe(dto);
    }

    private void SetupSender(Guid id, LevelDirectoryDto dto) =>
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<GetLevelDirectoryByIdQuery>(q => q.Id == id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dto);

    private static LevelDirectoryDto BuildDto(Guid id) =>
        new(id, "Beginner", 1, null, false, "BEGINNER", LevelTone.Slate);
}
