namespace Edvantix.Groups.UnitTests.Features.Directories.Levels.Create;

public sealed class CreateLevelDirectoryEndpointTests
{
    private readonly CreateLevelDirectoryEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();
    private readonly Mock<LinkGenerator> _linkerMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenDelegatesToSender()
    {
        var command = BuildCommand();
        var dto = BuildDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        await _endpoint.HandleAsync(command, _senderMock.Object, _linkerMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenReturnsCreatedWithDto()
    {
        var command = BuildCommand();
        var dto = BuildDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkerMock.Object);

        result.ShouldBeOfType<Created<LevelDirectoryDto>>();
        result.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenLocationContainsDtoId()
    {
        var command = BuildCommand();
        var dto = BuildDto();
        _senderMock.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkerMock.Object);

        result.Location!.ShouldContain(dto.Id.ToString());
    }

    private static CreateLevelDirectoryCommand BuildCommand() =>
        new("Beginner", Order: 1, Description: null);

    private static LevelDirectoryDto BuildDto() =>
        new(Guid.CreateVersion7(), "Beginner", 1, null, false, "BEGINNER", LevelTone.Slate);
}
