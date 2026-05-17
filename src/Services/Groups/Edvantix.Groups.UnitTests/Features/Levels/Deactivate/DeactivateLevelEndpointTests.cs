namespace Edvantix.Groups.UnitTests.Features.Levels.Deactivate;

public sealed class DeactivateLevelEndpointTests
{
    private readonly DeactivateLevelEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidId_WhenHandling_ThenSendsDeactivateCommandWithCorrectId()
    {
        var id = Guid.CreateVersion7();

        _senderMock
            .Setup(s => s.Send(It.IsAny<DeactivateLevelCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<DeactivateLevelCommand>(c => c.Id == id),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidId_WhenHandling_ThenReturnsNoContent()
    {
        var id = Guid.CreateVersion7();

        _senderMock
            .Setup(s => s.Send(It.IsAny<DeactivateLevelCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }
}
