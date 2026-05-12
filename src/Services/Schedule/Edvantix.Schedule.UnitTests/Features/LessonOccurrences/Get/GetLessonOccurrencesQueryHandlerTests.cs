namespace Edvantix.Schedule.UnitTests.Features.LessonOccurrences.Get;

public sealed class GetLessonOccurrencesQueryHandlerTests
{
    private readonly Mock<ILessonOccurrenceRepository> _repositoryMock = new();
    private readonly Mock<IMapper<LessonOccurrence, LessonOccurrenceDto>> _mapperMock = new();
    private readonly GetLessonOccurrencesQueryHandler _handler;

    public GetLessonOccurrencesQueryHandlerTests()
    {
        _handler = new(_repositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenOccurrences_WhenHandling_ThenShouldMapAndReturnDtos()
    {
        var occurrence = new LessonOccurrence(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            new DateOnly(2026, 1, 5),
            600,
            60
        );
        var dto = new LessonOccurrenceDto(
            occurrence.Id,
            occurrence.ScheduleId,
            occurrence.GroupId,
            occurrence.LessonDate,
            occurrence.StartMinutes,
            occurrence.DurationMinutes,
            occurrence.Status,
            occurrence.SkipReason,
            occurrence.LessonRefId
        );
        _repositoryMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<LessonOccurrence>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([occurrence]);
        _mapperMock.Setup(m => m.Map(occurrence)).Returns(dto);

        var result = await _handler.Handle(
            new GetLessonOccurrencesQuery(
                occurrence.GroupId,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 31)
            ),
            CancellationToken.None
        );

        result.ShouldHaveSingleItem();
        result[0].ShouldBe(dto);
        _mapperMock.Verify(m => m.Map(occurrence), Times.Once);
    }
}
