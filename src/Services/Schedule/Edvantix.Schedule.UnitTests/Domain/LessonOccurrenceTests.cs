namespace Edvantix.Schedule.UnitTests.Domain;

public sealed class LessonOccurrenceTests
{
    [Test]
    public void GivenValidData_WhenCreatingOccurrence_ThenShouldInitializeAsPlanned()
    {
        var scheduleId = Guid.CreateVersion7();
        var groupId = Guid.CreateVersion7();
        var date = new DateOnly(2026, 1, 5);

        var occurrence = new LessonOccurrence(scheduleId, groupId, date, 600, 60);

        occurrence.ScheduleId.ShouldBe(scheduleId);
        occurrence.GroupId.ShouldBe(groupId);
        occurrence.LessonDate.ShouldBe(date);
        occurrence.StartMinutes.ShouldBe(600);
        occurrence.DurationMinutes.ShouldBe((short)60);
        occurrence.Status.ShouldBe(OccurrenceStatus.Planned);
        occurrence.SkipReason.ShouldBeNull();
    }

    [Test]
    public void GivenOccurrence_WhenMarkingAsHeld_ThenStatusShouldBeHeld()
    {
        var occurrence = CreateOccurrence();

        occurrence.MarkAsHeld();

        occurrence.Status.ShouldBe(OccurrenceStatus.Held);
    }

    [Test]
    public void GivenOccurrence_WhenMarkingAsSkipped_ThenShouldTrimReason()
    {
        var occurrence = CreateOccurrence();

        occurrence.MarkAsSkipped("  sick leave  ");

        occurrence.Status.ShouldBe(OccurrenceStatus.Skipped);
        occurrence.SkipReason.ShouldBe("sick leave");
    }

    [Test]
    public void GivenOccurrence_WhenCancelling_ThenShouldTrimReason()
    {
        var occurrence = CreateOccurrence();

        occurrence.Cancel("  cancelled  ");

        occurrence.Status.ShouldBe(OccurrenceStatus.Cancelled);
        occurrence.SkipReason.ShouldBe("cancelled");
    }

    private static LessonOccurrence CreateOccurrence() =>
        new(Guid.CreateVersion7(), Guid.CreateVersion7(), new DateOnly(2026, 1, 5), 600, 60);
}
