namespace Edvantix.Organizational.UnitTests.Features.Groups.Get;

public sealed class GetGroupByIdEndpointTests
{
    private readonly GetGroupByIdEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldSendQueryWithCorrectId()
    {
        var id = Guid.CreateVersion7();
        var dto = CreateDto(id);
        _senderMock
            .Setup(s =>
                s.Send(It.Is<GetGroupByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(dto);

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.Is<GetGroupByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var id = Guid.CreateVersion7();
        var dto = CreateDto(id);
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetGroupByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.ShouldBeOfType<Ok<GroupDetailDto>>();
        result.Value.ShouldBe(dto);
    }

    private static GroupDetailDto CreateDto(Guid id) =>
        new(
            id,
            "B1-01",
            "Английский B1",
            "Описание",
            LevelId: Guid.CreateVersion7(),
            LevelCode: "B1",
            LevelName: "B1 — Средний",
            LevelTone: LevelTone.Blue,
            CourseId: Guid.CreateVersion7(),
            CourseCode: string.Empty,
            CourseName: string.Empty,
            Teacher: new TeacherDto(Guid.CreateVersion7(), string.Empty, string.Empty, null),
            RoomId: null,
            RoomLabel: null,
            Format: GroupFormat.Online,
            Platform: OnlinePlatform.Zoom,
            Schedule: null,
            UpcomingLessons: [],
            Capacity: 10,
            MemberCount: 0,
            Status: GroupStatus.Recruiting,
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30)
        );
}
