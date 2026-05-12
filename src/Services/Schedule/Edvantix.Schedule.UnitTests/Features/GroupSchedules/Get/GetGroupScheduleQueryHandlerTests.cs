namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.Get;

public sealed class GetGroupScheduleQueryHandlerTests
{
    private readonly Mock<IGroupScheduleRepository> _repositoryMock = new();
    private readonly Mock<IMapper<GroupSchedule, GroupScheduleDto>> _mapperMock = new();
    private readonly GetGroupScheduleQueryHandler _handler;

    public GetGroupScheduleQueryHandlerTests()
    {
        _handler = new(_repositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenMissingSchedule_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var query = new GetGroupScheduleQuery(Guid.CreateVersion7());
        _repositoryMock
            .Setup(r => r.GetByGroupIdAsync(query.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroupSchedule?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenExistingSchedule_WhenHandling_ThenShouldMapAndReturnDto()
    {
        var schedule = CreateSchedule();
        var dto = CreateDto(schedule);
        _repositoryMock
            .Setup(r => r.GetByGroupIdAsync(schedule.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);
        _mapperMock.Setup(m => m.Map(schedule)).Returns(dto);

        var result = await _handler.Handle(
            new GetGroupScheduleQuery(schedule.GroupId),
            CancellationToken.None
        );

        result.ShouldBe(dto);
        _mapperMock.Verify(m => m.Map(schedule), Times.Once);
    }

    private static GroupSchedule CreateSchedule() =>
        new(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            RecurrenceType.Weekly,
            60,
            new DateOnly(2026, 1, 5),
            EndMode.Count,
            null,
            2,
            null,
            false,
            true
        );

    private static GroupScheduleDto CreateDto(GroupSchedule schedule) =>
        new(
            schedule.Id,
            schedule.GroupId,
            schedule.OrganizationId,
            schedule.Recurrence,
            schedule.BiweeklyParity,
            schedule.LessonDurationMinutes,
            schedule.StartDate,
            schedule.EndMode,
            schedule.EndDate,
            schedule.LessonCount,
            schedule.SkipHolidays,
            schedule.NotifyStudents,
            [],
            []
        );
}
