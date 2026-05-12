namespace Edvantix.Curriculum.UnitTests.Features.Courses.Update;

public sealed class UpdateCourseEndpointTests
{
    private readonly UpdateCourseEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldSendCommand()
    {
        var command = new UpdateCourseCommand(Guid.CreateVersion7(), "Updated", null, "B2", 16);

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldReturnNoContent()
    {
        var command = new UpdateCourseCommand(Guid.CreateVersion7(), "Updated", null, "B2", 16);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<NoContent>();
    }
}
