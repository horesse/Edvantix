namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.RemoveException;

public sealed class RemoveScheduleExceptionCommandHandlerTests
{
    private readonly Mock<IGroupScheduleRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly RemoveScheduleExceptionCommandHandler _handler;

    public RemoveScheduleExceptionCommandHandlerTests()
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
        var command = new RemoveScheduleExceptionCommand(
            Guid.CreateVersion7(),
            Guid.CreateVersion7()
        );
        _repositoryMock
            .Setup(r => r.GetByGroupIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroupSchedule?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenExistingException_WhenHandling_ThenShouldRemoveAndSave()
    {
        var schedule = CreateSchedule();
        var exception = schedule.AddException(new DateOnly(2026, 1, 12));
        var command = new RemoveScheduleExceptionCommand(schedule.GroupId, exception.Id);
        _repositoryMock
            .Setup(r => r.GetByGroupIdAsync(command.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        await _handler.Handle(command, CancellationToken.None);

        schedule.Exceptions.ShouldBeEmpty();
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
