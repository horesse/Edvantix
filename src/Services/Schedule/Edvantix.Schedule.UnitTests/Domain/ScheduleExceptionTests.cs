namespace Edvantix.Schedule.UnitTests.Domain;

public sealed class ScheduleExceptionTests
{
    [Test]
    public void GivenValidData_WhenCreatingException_ThenShouldInitializeProperties()
    {
        var scheduleId = Guid.CreateVersion7();
        var date = new DateOnly(2026, 2, 1);

        var exception = new ScheduleException(scheduleId, date, "  break  ");

        exception.ScheduleId.ShouldBe(scheduleId);
        exception.ExceptionDate.ShouldBe(date);
        exception.Reason.ShouldBe("break");
    }
}
