namespace Edvantix.Curriculum.UnitTests.Domain.EventHandlers;

public sealed class CourseArchivedDomainEventHandlerTests
{
    private readonly Mock<IEventDispatcher> _dispatcherMock = new();

    [Test]
    public async Task GivenCourseArchivedEvent_WhenHandling_ThenShouldDispatchEvent()
    {
        var handler = new CourseArchivedDomainEventHandler(_dispatcherMock.Object);
        var @event = new CourseArchivedDomainEvent(Guid.CreateVersion7(), Guid.CreateVersion7());

        await handler.Handle(@event, CancellationToken.None);

        _dispatcherMock.Verify(
            d => d.DispatchAsync(@event, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenDispatcherFailure_WhenHandling_ThenShouldPropagateException()
    {
        var expected = new InvalidOperationException("dispatch failed");
        var @event = new CourseArchivedDomainEvent(Guid.CreateVersion7(), Guid.CreateVersion7());
        _dispatcherMock
            .Setup(d => d.DispatchAsync(@event, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expected);
        var handler = new CourseArchivedDomainEventHandler(_dispatcherMock.Object);

        var actual = await Should.ThrowAsync<InvalidOperationException>(() =>
            handler.Handle(@event, CancellationToken.None).AsTask()
        );

        actual.Message.ShouldBe(expected.Message);
    }
}
