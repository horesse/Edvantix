namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.Create;

public sealed class CreateGroupScheduleCommandHandlerTests
{
    private readonly Mock<IGroupScheduleRepository> _scheduleRepositoryMock = new();
    private readonly Mock<ILessonOccurrenceRepository> _occurrenceRepositoryMock = new();
    private readonly Mock<IHolidayRepository> _holidayRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CreateGroupScheduleCommandHandler _handler;

    public CreateGroupScheduleCommandHandlerTests()
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
    public async Task GivenExistingSchedule_WhenHandling_ThenShouldThrowInvalidOperationException()
    {
        var command = CreateCommand();
        _scheduleRepositoryMock
            .Setup(r => r.GetByGroupIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSchedule(command.GroupId, command.OrganizationId));

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldAddScheduleAndSave()
    {
        GroupSchedule? capturedSchedule = null;
        var command = CreateCommand(slots: []);
        _scheduleRepositoryMock
            .Setup(r => r.GetByGroupIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroupSchedule?)null);
        _scheduleRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<GroupSchedule>(), It.IsAny<CancellationToken>()))
            .Callback<GroupSchedule, CancellationToken>(
                (schedule, _) => capturedSchedule = schedule
            )
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBe(Guid.Empty);
        capturedSchedule.ShouldNotBeNull();
        capturedSchedule.GroupId.ShouldBe(command.GroupId);
        _scheduleRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<GroupSchedule>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenCommandWithSlots_WhenHandling_ThenShouldMaterializeOccurrences()
    {
        var command = CreateCommand();
        _scheduleRepositoryMock
            .Setup(r => r.GetByGroupIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroupSchedule?)null);
        _scheduleRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<GroupSchedule>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _occurrenceRepositoryMock
            .Setup(r =>
                r.AddRangeAsync(
                    It.IsAny<IEnumerable<LessonOccurrence>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

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
    public async Task GivenSkipHolidays_WhenHandling_ThenShouldLoadCurrentAndNextYearHolidays()
    {
        var command = CreateCommand(skipHolidays: true, holidayCountryCode: "BLR");
        _scheduleRepositoryMock
            .Setup(r => r.GetByGroupIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroupSchedule?)null);
        _scheduleRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<GroupSchedule>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _holidayRepositoryMock
            .Setup(r => r.GetByCountryAndYearAsync("BLR", 2026, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _holidayRepositoryMock
            .Setup(r => r.GetByCountryAndYearAsync("BLR", 2027, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await _handler.Handle(command, CancellationToken.None);

        _holidayRepositoryMock.Verify(
            r => r.GetByCountryAndYearAsync("BLR", 2026, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _holidayRepositoryMock.Verify(
            r => r.GetByCountryAndYearAsync("BLR", 2027, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private static CreateGroupScheduleCommand CreateCommand(
        IReadOnlyList<SlotRequest>? slots = null,
        bool skipHolidays = false,
        string? holidayCountryCode = null
    ) =>
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
            NotifyStudents: true,
            slots ?? [new SlotRequest(1, 600)],
            holidayCountryCode
        );

    private static GroupSchedule CreateSchedule(Guid groupId, Guid organizationId) =>
        new(
            groupId,
            organizationId,
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
