namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.Regenerate;

public sealed class RegenerateOccurrencesCommandHandlerTests
{
    private readonly Mock<IGroupScheduleRepository> _scheduleRepositoryMock = new();
    private readonly Mock<ILessonOccurrenceRepository> _occurrenceRepositoryMock = new();
    private readonly Mock<IHolidayRepository> _holidayRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly RegenerateOccurrencesCommandHandler _handler;

    public RegenerateOccurrencesCommandHandlerTests()
    {
        _scheduleRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(
            _scheduleRepositoryMock.Object,
            _occurrenceRepositoryMock.Object,
            _holidayRepositoryMock.Object
        );
    }

    [Test]
    public async Task GivenMissingSchedule_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var command = new RegenerateOccurrencesCommand(Guid.CreateVersion7(), null);
        _scheduleRepositoryMock
            .Setup(r => r.GetByGroupIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroupSchedule?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenScheduleWithoutSlots_WhenHandling_ThenShouldDeleteExistingAndSave()
    {
        var schedule = CreateSchedule();
        _scheduleRepositoryMock
            .Setup(r => r.GetByGroupIdAsync(schedule.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);
        _occurrenceRepositoryMock
            .Setup(r => r.DeleteByScheduleIdAsync(schedule.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(
            new RegenerateOccurrencesCommand(schedule.GroupId, null),
            CancellationToken.None
        );

        _occurrenceRepositoryMock.Verify(
            r => r.DeleteByScheduleIdAsync(schedule.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _occurrenceRepositoryMock.Verify(
            r =>
                r.AddRangeAsync(
                    It.IsAny<IEnumerable<LessonOccurrence>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenScheduleWithSlots_WhenHandling_ThenShouldAddNewOccurrences()
    {
        var schedule = CreateSchedule();
        schedule.ReplaceSlots([(1, 600)]);
        _scheduleRepositoryMock
            .Setup(r => r.GetByGroupIdAsync(schedule.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);
        _occurrenceRepositoryMock
            .Setup(r => r.DeleteByScheduleIdAsync(schedule.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _occurrenceRepositoryMock
            .Setup(r =>
                r.AddRangeAsync(
                    It.IsAny<IEnumerable<LessonOccurrence>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        await _handler.Handle(
            new RegenerateOccurrencesCommand(schedule.GroupId, null),
            CancellationToken.None
        );

        _occurrenceRepositoryMock.Verify(
            r =>
                r.AddRangeAsync(
                    It.Is<IEnumerable<LessonOccurrence>>(occurrences => occurrences.Any()),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenScheduleSkippingHolidays_WhenHandling_ThenShouldLoadHolidays()
    {
        var schedule = CreateSchedule(skipHolidays: true);
        schedule.ReplaceSlots([(1, 600)]);
        _scheduleRepositoryMock
            .Setup(r => r.GetByGroupIdAsync(schedule.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);
        _occurrenceRepositoryMock
            .Setup(r => r.DeleteByScheduleIdAsync(schedule.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _holidayRepositoryMock
            .Setup(r => r.GetByCountryAndYearAsync("BLR", 2026, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _holidayRepositoryMock
            .Setup(r => r.GetByCountryAndYearAsync("BLR", 2027, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await _handler.Handle(
            new RegenerateOccurrencesCommand(schedule.GroupId, "BLR"),
            CancellationToken.None
        );

        _holidayRepositoryMock.Verify(
            r => r.GetByCountryAndYearAsync("BLR", 2026, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _holidayRepositoryMock.Verify(
            r => r.GetByCountryAndYearAsync("BLR", 2027, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private static GroupSchedule CreateSchedule(bool skipHolidays = false) =>
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
            skipHolidays,
            true
        );
}
