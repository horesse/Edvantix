using Shouldly;

namespace Edvantix.Scheduling.UnitTests.Domain;

/// <summary>
/// Unit tests for the <see cref="LessonSlot"/> domain aggregate.
/// Covers constructor validation, Reschedule, and ChangeTeacher behaviors.
/// </summary>
public sealed class LessonSlotAggregateTests
{
    private static readonly Guid SchoolId = Guid.NewGuid();
    private static readonly Guid GroupId = Guid.NewGuid();
    private static readonly Guid TeacherId = Guid.NewGuid();
    private static readonly DateTimeOffset Start = new(2026, 9, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End = new(2026, 9, 1, 11, 0, 0, TimeSpan.Zero);

    [Test]
    public void GivenValidArguments_WhenCreatingLessonSlot_ThenPropertiesAreSet()
    {
        var slot = new LessonSlot(SchoolId, GroupId, TeacherId, Start, End);

        slot.SchoolId.ShouldBe(SchoolId);
        slot.GroupId.ShouldBe(GroupId);
        slot.TeacherId.ShouldBe(TeacherId);
        slot.StartTime.ShouldBe(Start);
        slot.EndTime.ShouldBe(End);
    }

    [Test]
    public void GivenEndTimeBeforeStartTime_WhenCreatingLessonSlot_ThenThrowsArgumentException()
    {
        Should
            .Throw<ArgumentException>(() =>
                new LessonSlot(SchoolId, GroupId, TeacherId, End, Start)
            )
            .Message.ShouldContain("EndTime must be strictly after StartTime");
    }

    [Test]
    public void GivenEndTimeEqualToStartTime_WhenCreatingLessonSlot_ThenThrowsArgumentException()
    {
        Should
            .Throw<ArgumentException>(() =>
                new LessonSlot(SchoolId, GroupId, TeacherId, Start, Start)
            )
            .Message.ShouldContain("EndTime must be strictly after StartTime");
    }

    [Test]
    public void GivenValidTimes_WhenRescheduling_ThenTimesAreUpdated()
    {
        var slot = new LessonSlot(SchoolId, GroupId, TeacherId, Start, End);
        var newStart = Start.AddDays(1);
        var newEnd = End.AddDays(1);

        slot.Reschedule(newStart, newEnd);

        slot.StartTime.ShouldBe(newStart);
        slot.EndTime.ShouldBe(newEnd);
    }

    [Test]
    public void GivenEndTimeBeforeStartTime_WhenRescheduling_ThenThrowsArgumentException()
    {
        var slot = new LessonSlot(SchoolId, GroupId, TeacherId, Start, End);

        Should
            .Throw<ArgumentException>(() => slot.Reschedule(End, Start))
            .Message.ShouldContain("EndTime must be strictly after StartTime");
    }

    [Test]
    public void GivenValidTeacherId_WhenChangingTeacher_ThenTeacherIdIsUpdated()
    {
        var slot = new LessonSlot(SchoolId, GroupId, TeacherId, Start, End);
        var newTeacherId = Guid.NewGuid();

        slot.ChangeTeacher(newTeacherId);

        slot.TeacherId.ShouldBe(newTeacherId);
    }

    [Test]
    public void GivenDefaultTeacherId_WhenChangingTeacher_ThenThrowsArgumentException()
    {
        var slot = new LessonSlot(SchoolId, GroupId, TeacherId, Start, End);

        Should.Throw<ArgumentException>(() => slot.ChangeTeacher(Guid.Empty));
    }
}
