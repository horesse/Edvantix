namespace Edvantix.Curriculum.UnitTests.Features.Lessons.Publish;

public sealed class PublishLessonEndpointTests
{
    private readonly PublishLessonEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenLessonId_WhenHandling_ThenShouldSendPublishCommand()
    {
        var id = Guid.CreateVersion7();

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<PublishLessonCommand>(c => c.LessonId == id),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenLessonId_WhenHandling_ThenShouldReturnNoContent()
    {
        var result = await _endpoint.HandleAsync(Guid.CreateVersion7(), _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }
}
