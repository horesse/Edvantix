namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.AddException;

public sealed class AddScheduleExceptionCommandHandlerTests
{
    private readonly Mock<IGroupScheduleRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly AddScheduleExceptionCommandHandler _handler;

    public AddScheduleExceptionCommandHandlerTests()
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
        var command = new AddScheduleExceptionCommand(
            Guid.CreateVersion7(),
            new DateOnly(2026, 1, 5),
            null
        );
        _repositoryMock
            .Setup(r => r.GetByGroupIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroupSchedule?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenExistingSchedule_WhenHandling_ThenShouldAddExceptionAndSave()
    {
        var schedule = CreateSchedule();
        var command = new AddScheduleExceptionCommand(
            schedule.GroupId,
            new DateOnly(2026, 1, 12),
            "reason"
        );
        _repositoryMock
            .Setup(r => r.GetByGroupIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBe(Guid.Empty);
        schedule.Exceptions.ShouldHaveSingleItem();
        schedule.Exceptions[0].ExceptionDate.ShouldBe(command.ExceptionDate);
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
}
