namespace Edvantix.Groups.Grpc.Services.Schedules;

/// <summary>Предстоящее занятие группы.</summary>
public sealed record UpcomingLessonDto(
    [property: Description("Дата занятия")] DateOnly Date,
    [property: Description("Время начала занятия")] TimeOnly StartTime,
    [property: Description("Время окончания занятия")] TimeOnly EndTime
);
