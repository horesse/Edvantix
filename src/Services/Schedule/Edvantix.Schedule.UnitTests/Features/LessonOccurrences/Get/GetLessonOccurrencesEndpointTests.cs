namespace Edvantix.Schedule.UnitTests.Features.LessonOccurrences.Get;

public sealed class GetLessonOccurrencesEndpointTests
{
    private readonly GetLessonOccurrencesEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldSendQuery()
    {
        var groupId = Guid.CreateVersion7();
        var from = new DateOnly(2026, 1, 1);
        var to = new DateOnly(2026, 1, 31);
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<GetLessonOccurrencesQuery>(q =>
                        q.GroupId == groupId && q.From == from && q.To == to
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);

        await _endpoint.HandleAsync((groupId, from, to), _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<GetLessonOccurrencesQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldReturnOk()
    {
        IReadOnlyList<LessonOccurrenceDto> expected =
        [
            new(
                Guid.CreateVersion7(),
                Guid.CreateVersion7(),
                Guid.CreateVersion7(),
                new DateOnly(2026, 1, 5),
                600,
                60,
                OccurrenceStatus.Planned,
                null,
                null
            ),
        ];
        _senderMock
            .Setup(s =>
                s.Send(It.IsAny<GetLessonOccurrencesQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(expected);

        var result = await _endpoint.HandleAsync(
            (Guid.CreateVersion7(), new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 31)),
            _senderMock.Object
        );

        result.ShouldBeOfType<Ok<IReadOnlyList<LessonOccurrenceDto>>>();
        result.Value.ShouldBe(expected);
    }
}
