namespace Edvantix.Curriculum.UnitTests.Features.Courses.Options;

public sealed class GetCourseOptionsEndpointTests
{
    private readonly GetCourseOptionsEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldSendQuery()
    {
        var query = new GetCourseOptionsQuery("en");
        IReadOnlyList<CourseOptionDto> options = [BuildOption()];
        _senderMock.Setup(s => s.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(options);

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldReturnOkWithOptions()
    {
        var query = new GetCourseOptionsQuery();
        IReadOnlyList<CourseOptionDto> options = [BuildOption()];
        _senderMock.Setup(s => s.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(options);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.Value.ShouldBe(options);
    }

    private static CourseOptionDto BuildOption() =>
        new(Guid.CreateVersion7(), "EN", "English", "B1", CourseSubject.English);
}
