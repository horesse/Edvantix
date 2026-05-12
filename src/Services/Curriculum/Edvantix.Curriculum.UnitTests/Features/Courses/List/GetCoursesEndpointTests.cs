namespace Edvantix.Curriculum.UnitTests.Features.Courses.List;

public sealed class GetCoursesEndpointTests
{
    private readonly GetCoursesEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldSendQuery()
    {
        var query = new GetCoursesQuery(Search: "english");
        var result = new PagedResult<CourseDto>([], 1, 10, 0);
        _senderMock.Setup(s => s.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(result);

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenQuery_WhenHandling_ThenShouldReturnOkWithPagedResult()
    {
        var query = new GetCoursesQuery();
        var result = new PagedResult<CourseDto>([], 1, 10, 0);
        _senderMock.Setup(s => s.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var response = await _endpoint.HandleAsync(query, _senderMock.Object);

        response.Value.ShouldBe(result);
    }
}
