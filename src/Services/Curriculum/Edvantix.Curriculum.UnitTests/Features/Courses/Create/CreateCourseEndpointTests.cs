namespace Edvantix.Curriculum.UnitTests.Features.Courses.Create;

public sealed class CreateCourseEndpointTests
{
    private readonly CreateCourseEndpoint _endpoint = new();
    private readonly LinkGenerator _linkGenerator = new Mock<LinkGenerator>().Object;
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldCallSenderOnce()
    {
        var command = BuildValidCommand();
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnCreated()
    {
        var command = BuildValidCommand();
        var expectedId = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        result.Value.ShouldBe(expectedId);
        result.Location!.ShouldContain(expectedId.ToString());
    }

    private static CreateCourseCommand BuildValidCommand() =>
        new(
            "EN-GEN-B1",
            "English General B1",
            CourseSubject.English,
            "B1",
            12,
            Guid.CreateVersion7()
        );
}
