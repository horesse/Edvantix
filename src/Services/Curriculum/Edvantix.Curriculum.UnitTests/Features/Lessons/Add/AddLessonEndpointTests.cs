namespace Edvantix.Curriculum.UnitTests.Features.Lessons.Add;

public sealed class AddLessonEndpointTests
{
    private readonly AddLessonEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldSendCommand()
    {
        var command = new AddLessonCommand(
            Guid.CreateVersion7(),
            "Lesson",
            LessonType.Lecture,
            45,
            []
        );
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        await _endpoint.HandleAsync(command, _senderMock.Object);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenCommand_WhenHandling_ThenShouldReturnCreated()
    {
        var command = new AddLessonCommand(
            Guid.CreateVersion7(),
            "Lesson",
            LessonType.Lecture,
            45,
            []
        );
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.Value.ShouldBe(expectedId);
        var location = result.Location;
        location.ShouldNotBeNull();
        location.ShouldContain(command.ModuleId.ToString());
        location.ShouldContain(expectedId.ToString());
    }
}
