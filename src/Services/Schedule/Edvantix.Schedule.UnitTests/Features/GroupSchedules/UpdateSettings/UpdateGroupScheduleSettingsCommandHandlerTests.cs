namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.UpdateSettings;

public sealed class UpdateGroupScheduleSettingsCommandHandlerTests
{
    private readonly Mock<IGroupScheduleRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly UpdateGroupScheduleSettingsCommandHandler _handler;

    public UpdateGroupScheduleSettingsCommandHandlerTests()
    {
        _repositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(_repositoryMock.Object);
    }

    [Test]
    public async Task GivenMissingSchedule_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var command = CreateCommand();
        _repositoryMock
            .Setup(r => r.GetByGroupIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroupSchedule?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenExistingSchedule_WhenHandling_ThenShouldUpdateSettingsAndSlots()
    {
        var command = CreateCommand();
        var schedule = CreateSchedule(command.GroupId);
        _repositoryMock
            .Setup(r => r.GetByGroupIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        await _handler.Handle(command, CancellationToken.None);

        schedule.Recurrence.ShouldBe(command.Recurrence);
        schedule.LessonDurationMinutes.ShouldBe(command.LessonDurationMinutes);
        schedule.Slots.Count.ShouldBe(command.Slots.Count);
        schedule.Slots[0].StartMinutes.ShouldBe(720);
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static UpdateGroupScheduleSettingsCommand CreateCommand() =>
        new(
            Guid.CreateVersion7(),
            RecurrenceType.Weekly,
            90,
            EndMode.Count,
            null,
            2,
            null,
            SkipHolidays: true,
            NotifyStudents: false,
            [new SlotRequest(1, 720)]
        );

    private static GroupSchedule CreateSchedule(Guid groupId) =>
        new(
            groupId,
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
}
