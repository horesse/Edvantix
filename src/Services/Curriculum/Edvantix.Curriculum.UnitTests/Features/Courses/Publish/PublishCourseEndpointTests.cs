namespace Edvantix.Curriculum.UnitTests.Features.Courses.Publish;

public sealed class PublishCourseEndpointTests
{
    private readonly PublishCourseEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenCourseId_WhenHandling_ThenShouldSendPublishCommand()
    {
        var id = Guid.CreateVersion7();

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<PublishCourseCommand>(c => c.CourseId == id),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenCourseId_WhenHandling_ThenShouldReturnNoContent()
    {
        var result = await _endpoint.HandleAsync(Guid.CreateVersion7(), _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }
}
