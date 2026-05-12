namespace Edvantix.Curriculum.UnitTests.Infrastructure;

public sealed class EventMapperTests
{
    private readonly EventMapper _mapper = new();

    [Test]
    public void GivenCourseArchivedDomainEvent_WhenMapping_ThenShouldReturnCourseArchivedIntegrationEvent()
    {
        var @event = new CourseArchivedDomainEvent(Guid.CreateVersion7(), Guid.CreateVersion7());

        var result = _mapper.MapToIntegrationEvent(@event);

        var integrationEvent = result.ShouldBeOfType<CourseArchivedIntegrationEvent>();
        integrationEvent.CourseId.ShouldBe(@event.CourseId);
        integrationEvent.OrganizationId.ShouldBe(@event.OrganizationId);
    }

    [Test]
    public void GivenUnsupportedDomainEvent_WhenMapping_ThenShouldThrowArgumentOutOfRangeException()
    {
        var @event = new UnsupportedDomainEvent();

        var act = () => _mapper.MapToIntegrationEvent(@event);

        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    private sealed class UnsupportedDomainEvent : DomainEvent;
}
