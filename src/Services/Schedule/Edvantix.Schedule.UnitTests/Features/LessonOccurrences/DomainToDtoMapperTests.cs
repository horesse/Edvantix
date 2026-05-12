namespace Edvantix.Schedule.UnitTests.Features.LessonOccurrences;

public sealed class DomainToDtoMapperTests
{
    private readonly DomainToDtoMapper _mapper = new();

    [Test]
    public void GivenOccurrence_WhenMapping_ThenShouldMapAllProperties()
    {
        var occurrence = new LessonOccurrence(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            new DateOnly(2026, 1, 5),
            600,
            60
        );
        occurrence.MarkAsSkipped("reason");

        var dto = _mapper.Map(occurrence);

        dto.Id.ShouldBe(occurrence.Id);
        dto.ScheduleId.ShouldBe(occurrence.ScheduleId);
        dto.GroupId.ShouldBe(occurrence.GroupId);
        dto.LessonDate.ShouldBe(occurrence.LessonDate);
        dto.StartMinutes.ShouldBe(600);
        dto.DurationMinutes.ShouldBe(60);
        dto.Status.ShouldBe(OccurrenceStatus.Skipped);
        dto.SkipReason.ShouldBe("reason");
    }
}
