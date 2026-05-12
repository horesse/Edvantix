namespace Edvantix.Curriculum.UnitTests.Features.Courses.Get;

public sealed class GetCourseByIdEndpointTests
{
    private readonly GetCourseByIdEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenCourseId_WhenHandling_ThenShouldSendQuery()
    {
        var id = Guid.CreateVersion7();
        var dto = BuildDto(id);
        _senderMock
            .Setup(s =>
                s.Send(It.Is<GetCourseByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(dto);

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.Is<GetCourseByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenCourseId_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var id = Guid.CreateVersion7();
        var dto = BuildDto(id);
        _senderMock
            .Setup(s =>
                s.Send(It.Is<GetCourseByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.Value.ShouldBe(dto);
    }

    private static CourseDetailDto BuildDto(Guid id) =>
        new(
            id,
            "EN",
            "English",
            CourseSubject.English,
            "B1",
            12,
            null,
            null,
            CourseStatus.Draft,
            Guid.CreateVersion7(),
            [],
            []
        );
}
