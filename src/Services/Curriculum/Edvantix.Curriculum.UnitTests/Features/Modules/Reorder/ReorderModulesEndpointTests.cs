namespace Edvantix.Curriculum.UnitTests.Features.Modules.Reorder;

public sealed class ReorderModulesEndpointTests
{
    private readonly ReorderModulesEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldSendCommand()
    {
        var command = new ReorderModulesCommand(Guid.CreateVersion7(), [Guid.CreateVersion7()]);

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldReturnNoContent()
    {
        var command = new ReorderModulesCommand(Guid.CreateVersion7(), []);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }
}
