namespace Edvantix.Organizational.UnitTests.Features.Levels.Update;

public sealed class UpdateLevelEndpointTests
{
    private readonly UpdateLevelEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenCallsSenderOnce()
    {
        var command = BuildValidCommand();

        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenReturnsNoContent()
    {
        var command = BuildValidCommand();

        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }

    private static UpdateLevelCommand BuildValidCommand() =>
        new(
            Id: Guid.CreateVersion7(),
            Name: "Advanced",
            Description: "Продвинутый уровень",
            Tone: LevelTone.Red,
            SortOrder: 5
        );
}
