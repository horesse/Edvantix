namespace Edvantix.Curriculum.UnitTests.Domain.EventHandlers;

public sealed class DomainEventNoOpHandlerTests
{
    [Test]
    public async Task GivenCourseCreatedEvent_WhenHandling_ThenShouldCompleteSuccessfully()
    {
        var handler = new CourseCreatedDomainEventHandler();
        var @event = new CourseCreatedDomainEvent(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7()
        );

        await handler.Handle(@event, CancellationToken.None);
    }

    [Test]
    public async Task GivenCoursePublishedEvent_WhenHandling_ThenShouldCompleteSuccessfully()
    {
        var handler = new CoursePublishedDomainEventHandler();
        var @event = new CoursePublishedDomainEvent(Guid.CreateVersion7(), Guid.CreateVersion7());

        await handler.Handle(@event, CancellationToken.None);
    }

    [Test]
    public async Task GivenLessonPublishedEvent_WhenHandling_ThenShouldCompleteSuccessfully()
    {
        var handler = new LessonPublishedDomainEventHandler();
        var @event = new LessonPublishedDomainEvent(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7()
        );

        await handler.Handle(@event, CancellationToken.None);
    }
}
