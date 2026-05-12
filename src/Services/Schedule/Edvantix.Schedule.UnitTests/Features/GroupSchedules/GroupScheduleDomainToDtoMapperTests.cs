namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules;

public sealed class GroupScheduleDomainToDtoMapperTests
{
    private readonly GroupScheduleDomainToDtoMapper _mapper = new();

    [Test]
    public void GivenSchedule_WhenMapping_ThenShouldMapAllProperties()
    {
        var schedule = new GroupSchedule(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            RecurrenceType.Weekly,
            60,
            new DateOnly(2026, 1, 5),
            EndMode.Count,
            null,
            2,
            null,
            skipHolidays: true,
            notifyStudents: false
        );
        schedule.ReplaceSlots([(1, 600)]);
        schedule.AddException(new DateOnly(2026, 1, 12), "reason");

        var dto = _mapper.Map(schedule);

        dto.Id.ShouldBe(schedule.Id);
        dto.GroupId.ShouldBe(schedule.GroupId);
        dto.OrganizationId.ShouldBe(schedule.OrganizationId);
        dto.Recurrence.ShouldBe(schedule.Recurrence);
        dto.LessonDurationMinutes.ShouldBe(schedule.LessonDurationMinutes);
        dto.Slots.ShouldHaveSingleItem();
        dto.Slots[0].Weekday.ShouldBe(1);
        dto.Exceptions.ShouldHaveSingleItem();
        dto.Exceptions[0].Reason.ShouldBe("reason");
    }
}
