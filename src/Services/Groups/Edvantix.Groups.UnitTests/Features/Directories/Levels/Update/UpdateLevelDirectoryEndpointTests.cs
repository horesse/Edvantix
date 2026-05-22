namespace Edvantix.Groups.UnitTests.Features.Directories.Levels.Update;

public sealed class UpdateLevelDirectoryEndpointTests
{
    private readonly UpdateLevelDirectoryEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenExistingLevel_WhenHandling_ThenDelegatesToSender()
    {
        var command = BuildCommand();
        SetupSender(command, BuildDto(command.Id));

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenExistingLevel_WhenHandling_ThenReturnsOkWithDto()
    {
        var command = BuildCommand();
        var dto = BuildDto(command.Id);
        SetupSender(command, dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        var ok = result.Result.ShouldBeOfType<Ok<LevelDirectoryDto>>();
        ok.Value.ShouldBe(dto);
    }

    private void SetupSender(UpdateLevelDirectoryCommand command, LevelDirectoryDto dto) =>
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

    private static UpdateLevelDirectoryCommand BuildCommand() =>
        new(Guid.CreateVersion7(), "Beginner", Order: 1, Description: null);

    private static LevelDirectoryDto BuildDto(Guid id) =>
        new(id, "Beginner", 1, null, false, "BEGINNER", LevelTone.Slate);
}
