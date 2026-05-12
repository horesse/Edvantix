namespace Edvantix.Schedule.UnitTests.Domain;

public sealed class ScheduleSlotTests
{
    [Test]
    public void GivenValidData_WhenCreatingSlot_ThenShouldInitializeProperties()
    {
        var scheduleId = Guid.CreateVersion7();

        var slot = new ScheduleSlot(scheduleId, 1, 600);

        slot.ScheduleId.ShouldBe(scheduleId);
        slot.Weekday.ShouldBe(1);
        slot.StartMinutes.ShouldBe(600);
    }

    [Test]
    [Arguments(-1)]
    [Arguments(7)]
    public void GivenInvalidWeekday_WhenCreatingSlot_ThenShouldThrow(int weekday)
    {
        var act = () => new ScheduleSlot(Guid.CreateVersion7(), weekday, 600);

        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments(-1)]
    [Arguments(1440)]
    public void GivenInvalidStartMinutes_WhenCreatingSlot_ThenShouldThrow(int startMinutes)
    {
        var act = () => new ScheduleSlot(Guid.CreateVersion7(), 1, startMinutes);

        act.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
