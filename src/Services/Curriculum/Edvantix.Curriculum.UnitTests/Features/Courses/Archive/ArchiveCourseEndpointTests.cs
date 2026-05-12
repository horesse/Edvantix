namespace Edvantix.Curriculum.UnitTests.Features.Courses.Archive;

public sealed class ArchiveCourseEndpointTests
{
    private readonly ArchiveCourseEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenCourseId_WhenHandling_ThenShouldSendArchiveCommand()
    {
        var id = Guid.CreateVersion7();

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<ArchiveCourseCommand>(c => c.CourseId == id),
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
