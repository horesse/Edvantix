namespace Edvantix.Curriculum.UnitTests.Features.Lessons.Move;

public sealed class MoveLessonEndpointTests
{
    private readonly MoveLessonEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldSendCommand()
    {
        var command = new MoveLessonCommand(Guid.CreateVersion7(), 2);

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldReturnNoContent()
    {
        var command = new MoveLessonCommand(Guid.CreateVersion7(), 2);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }
}
