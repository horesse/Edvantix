namespace Edvantix.Organizational.UnitTests.Features.Groups.ChangeStatus;

public sealed class ChangeGroupStatusEndpointTests
{
    private readonly ChangeGroupStatusEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
    {
        var command = new ChangeGroupStatusCommand(Guid.CreateVersion7(), GroupStatus.Active);
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(command, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnNoContent()
    {
        var command = new ChangeGroupStatusCommand(Guid.CreateVersion7(), GroupStatus.Active);
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }
}
