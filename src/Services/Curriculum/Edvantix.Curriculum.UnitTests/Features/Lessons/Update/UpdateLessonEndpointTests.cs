namespace Edvantix.Curriculum.UnitTests.Features.Lessons.Update;

public sealed class UpdateLessonEndpointTests
{
    private readonly UpdateLessonEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldSendCommand()
    {
        var command = new UpdateLessonCommand(
            Guid.CreateVersion7(),
            "Title",
            LessonType.Lecture,
            45,
            []
        );

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldReturnNoContent()
    {
        var command = new UpdateLessonCommand(
            Guid.CreateVersion7(),
            "Title",
            LessonType.Lecture,
            45,
            []
        );

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }
}
