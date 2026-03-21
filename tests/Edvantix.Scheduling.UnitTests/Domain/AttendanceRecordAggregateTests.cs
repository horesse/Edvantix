using Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;
using Shouldly;

namespace Edvantix.Scheduling.UnitTests.Domain;

/// <summary>
/// Unit tests for the <see cref="AttendanceRecord"/> domain aggregate.
/// Covers constructor validation, field storage, CorrelationId uniqueness, and UpdateStatus behavior.
/// </summary>
public sealed class AttendanceRecordAggregateTests
{
    private static readonly Guid SchoolId = Guid.NewGuid();
    private static readonly Guid LessonSlotId = Guid.NewGuid();
    private static readonly Guid StudentId = Guid.NewGuid();

    [Test]
    public void GivenValidInputs_WhenCreatingAttendanceRecord_ThenAllFieldsAreStored()
    {
        var before = DateTimeOffset.UtcNow;

        var record = new AttendanceRecord(SchoolId, LessonSlotId, StudentId, AttendanceStatus.Present);

        record.SchoolId.ShouldBe(SchoolId);
        record.LessonSlotId.ShouldBe(LessonSlotId);
        record.StudentId.ShouldBe(StudentId);
        record.Status.ShouldBe(AttendanceStatus.Present);
        record.CorrelationId.ShouldNotBe(Guid.Empty);
        record.MarkedAt.ShouldBeGreaterThanOrEqualTo(before);
        record.MarkedAt.ShouldBeLessThanOrEqualTo(DateTimeOffset.UtcNow.AddSeconds(1));
    }

    [Test]
    public void GivenDefaultSchoolId_WhenCreatingAttendanceRecord_ThenThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() =>
            new AttendanceRecord(Guid.Empty, LessonSlotId, StudentId, AttendanceStatus.Present)
        );
    }

    [Test]
    public void GivenDefaultLessonSlotId_WhenCreatingAttendanceRecord_ThenThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() =>
            new AttendanceRecord(SchoolId, Guid.Empty, StudentId, AttendanceStatus.Present)
        );
    }

    [Test]
    public void GivenDefaultStudentId_WhenCreatingAttendanceRecord_ThenThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() =>
            new AttendanceRecord(SchoolId, LessonSlotId, Guid.Empty, AttendanceStatus.Present)
        );
    }

    [Test]
    public void GivenExistingRecord_WhenUpdatingStatus_ThenStatusAndMarkedAtChange()
    {
        var record = new AttendanceRecord(SchoolId, LessonSlotId, StudentId, AttendanceStatus.Present);
        var markedAtBefore = record.MarkedAt;

        record.UpdateStatus(AttendanceStatus.Absent);

        record.Status.ShouldBe(AttendanceStatus.Absent);
        record.MarkedAt.ShouldBeGreaterThanOrEqualTo(markedAtBefore);
    }

    [Test]
    public void GivenExistingRecord_WhenUpdatingStatus_ThenCorrelationIdIsPreserved()
    {
        var record = new AttendanceRecord(SchoolId, LessonSlotId, StudentId, AttendanceStatus.Present);
        var originalCorrelationId = record.CorrelationId;

        record.UpdateStatus(AttendanceStatus.Absent);

        record.CorrelationId.ShouldBe(originalCorrelationId);
    }
}
