namespace Edvantix.Organizational.UnitTests.Features.Levels.Reorder;

public sealed class ReorderLevelsEndpointTests
{
    private readonly ReorderLevelsEndpoint _endpoint = new();
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

    private static ReorderLevelsCommand BuildValidCommand() =>
        new(
            [
                new LevelOrderItem(Guid.CreateVersion7(), SortOrder: 1),
                new LevelOrderItem(Guid.CreateVersion7(), SortOrder: 2),
            ]
        );
}
