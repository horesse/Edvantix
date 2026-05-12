namespace Edvantix.Schedule.UnitTests.IntegrationEvents.EventHandlers;

public sealed class GroupCreatedIntegrationEventHandlerTests
{
    private readonly Mock<IGroupScheduleRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly GroupCreatedIntegrationEventHandler _handler;

    public GroupCreatedIntegrationEventHandlerTests()
    {
        _repositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(_repositoryMock.Object);
    }

    [Test]
    public async Task GivenExistingSchedule_WhenHandling_ThenShouldDoNothing()
    {
        var @event = CreateEvent();
        _repositoryMock
            .Setup(r => r.GetByGroupIdAsync(@event.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSchedule(@event.GroupId, @event.OrganizationId));

        await _handler.Handle(@event, CancellationToken.None);

        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<GroupSchedule>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenNewGroup_WhenHandling_ThenShouldCreateEmptyScheduleAndSave()
    {
        GroupSchedule? capturedSchedule = null;
        var @event = CreateEvent();
        _repositoryMock
            .Setup(r => r.GetByGroupIdAsync(@event.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroupSchedule?)null);
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<GroupSchedule>(), It.IsAny<CancellationToken>()))
            .Callback<GroupSchedule, CancellationToken>(
                (schedule, _) => capturedSchedule = schedule
            )
            .Returns(Task.CompletedTask);

        await _handler.Handle(@event, CancellationToken.None);

        capturedSchedule.ShouldNotBeNull();
        capturedSchedule.GroupId.ShouldBe(@event.GroupId);
        capturedSchedule.OrganizationId.ShouldBe(@event.OrganizationId);
        capturedSchedule.StartDate.ShouldBe(@event.StartDate);
        capturedSchedule.Slots.ShouldBeEmpty();
        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static GroupCreatedIntegrationEvent CreateEvent() =>
        new()
        {
            GroupId = Guid.CreateVersion7(),
            OrganizationId = Guid.CreateVersion7(),
            StartDate = new DateOnly(2026, 1, 5),
        };

    private static GroupSchedule CreateSchedule(Guid groupId, Guid organizationId) =>
        GroupSchedule.CreateEmpty(groupId, organizationId, new DateOnly(2026, 1, 5));
}
