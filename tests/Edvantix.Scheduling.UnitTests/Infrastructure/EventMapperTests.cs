using Edvantix.Contracts;
using Edvantix.Scheduling.Infrastructure.EventServices;
using Edvantix.Scheduling.Infrastructure.EventServices.Events;
using Shouldly;

namespace Edvantix.Scheduling.UnitTests.Infrastructure;

/// <summary>
/// Unit tests for <see cref="EventMapper"/> in the Scheduling service.
/// Verifies that AttendanceRecordedEvent maps correctly to AttendanceRecordedIntegrationEvent.
/// </summary>
public sealed class EventMapperTests
{
    private readonly EventMapper _mapper = new();

    [Test]
    public void GivenAttendanceRecordedEvent_WhenMapping_ThenAllFieldsAreMapped()
    {
        var studentId = Guid.NewGuid();
        var lessonSlotId = Guid.NewGuid();
        var schoolId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var markedAt = DateTimeOffset.UtcNow;

        var domainEvent = new AttendanceRecordedEvent(
            correlationId,
            studentId,
            lessonSlotId,
            schoolId,
            "Present",
            markedAt
        );

        var result = (AttendanceRecordedIntegrationEvent)_mapper.MapToIntegrationEvent(domainEvent);

        result.StudentId.ShouldBe(studentId);
        result.LessonSlotId.ShouldBe(lessonSlotId);
        result.SchoolId.ShouldBe(schoolId);
        result.CorrelationId.ShouldBe(correlationId);
        result.Timestamp.ShouldBe(markedAt);
        result.Status.ShouldBe("Present");
    }

    [Test]
    public void GivenAttendanceRecordedEvent_WhenMapping_ThenStatusIsString()
    {
        var domainEvent = new AttendanceRecordedEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Present",
            DateTimeOffset.UtcNow
        );

        var result = (AttendanceRecordedIntegrationEvent)_mapper.MapToIntegrationEvent(domainEvent);

        // Status should be the string "Present", not "0" (the int enum value)
        result.Status.ShouldBe("Present");
        result.Status.ShouldNotBe("0");
    }

    [Test]
    public void GivenAttendanceRecordedEvent_WhenMapping_ThenCorrelationIdIsPreserved()
    {
        var correlationId = Guid.NewGuid();

        var domainEvent = new AttendanceRecordedEvent(
            correlationId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Absent",
            DateTimeOffset.UtcNow
        );

        var result = (AttendanceRecordedIntegrationEvent)_mapper.MapToIntegrationEvent(domainEvent);

        result.CorrelationId.ShouldBe(correlationId);
    }

    [Test]
    public void GivenUnknownDomainEvent_WhenMapping_ThenThrowsArgumentOutOfRangeException()
    {
        var unknownEvent = new UnknownDomainEventStub();

        Should.Throw<ArgumentOutOfRangeException>(() =>
            _mapper.MapToIntegrationEvent(unknownEvent)
        );
    }

    /// <summary>Stub domain event used to verify the default throw case in EventMapper.</summary>
    private sealed class UnknownDomainEventStub : DomainEvent { }
}
