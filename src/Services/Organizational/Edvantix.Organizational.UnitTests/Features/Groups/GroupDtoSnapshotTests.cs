namespace Edvantix.Organizational.UnitTests.Features.Groups;

/// <summary>
/// Snapshot-тесты JSON-контракта DTO.
/// При первом запуске генерируют файлы *.received.txt; после проверки
/// их нужно переименовать в *.verified.txt (или принять через diff tool).
/// </summary>
public sealed class GroupDtoSnapshotTests
{
    private static readonly Guid FixedId = new("11111111-0000-7000-8000-000000000001");
    private static readonly Guid FixedLevelId = new("22222222-0000-7000-8000-000000000002");
    private static readonly Guid FixedCourseId = new("33333333-0000-7000-8000-000000000003");
    private static readonly Guid FixedTeacherId = new("44444444-0000-7000-8000-000000000004");
    private static readonly Guid FixedScheduleId = new("55555555-0000-7000-8000-000000000005");

    [Test]
    public Task GivenGroupListItemDto_WhenSerialized_ThenSnapshotMatchesContract()
    {
        var dto = new GroupListItemDto(
            Id: FixedId,
            Code: "B1-01",
            Name: "Английский B1",
            LevelId: FixedLevelId,
            LevelCode: "B1",
            LevelName: "B1 — Средний",
            LevelTone: LevelTone.Blue,
            CourseId: FixedCourseId,
            CourseCode: "EN-GEN-B1",
            CourseName: "Английский Общий B1",
            Teacher: new TeacherDto(FixedTeacherId, "Иванова Мария", "Преподаватель", null),
            RoomId: null,
            RoomLabel: null,
            Format: GroupFormat.Online,
            Platform: OnlinePlatform.Zoom,
            ScheduleSummary: null,
            Capacity: 12,
            MemberCount: 8,
            Status: GroupStatus.Active,
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30)
        );

        return Verify(dto);
    }

    [Test]
    public Task GivenGroupDetailDto_WhenSerialized_ThenSnapshotMatchesContract()
    {
        var dto = new GroupDetailDto(
            Id: FixedId,
            Code: "B1-01",
            Name: "Английский B1",
            Description: "Группа для уровня B1",
            LevelId: FixedLevelId,
            LevelCode: "B1",
            LevelName: "B1 — Средний",
            LevelTone: LevelTone.Blue,
            CourseId: FixedCourseId,
            CourseCode: "EN-GEN-B1",
            CourseName: "Английский Общий B1",
            Teacher: new TeacherDto(FixedTeacherId, "Иванова Мария", "Преподаватель", null),
            RoomId: null,
            RoomLabel: null,
            Format: GroupFormat.Online,
            Platform: OnlinePlatform.Zoom,
            Schedule: null,
            UpcomingLessons: [],
            Capacity: 12,
            MemberCount: 8,
            Status: GroupStatus.Active,
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30)
        );

        return Verify(dto);
    }

    [Test]
    public Task GivenGroupDetailDtoWithEmptySchedule_WhenSerialized_ThenSnapshotMatchesContract()
    {
        var schedule = new ScheduleDetailDto(
            Id: FixedScheduleId,
            Recurrence: "Weekly",
            BiweeklyParity: null,
            LessonDurationMinutes: 60,
            StartDate: new DateOnly(2025, 9, 1),
            EndMode: "Date",
            EndDate: new DateOnly(2026, 6, 30),
            LessonCount: null,
            SkipHolidays: false,
            Slots: [],
            Exceptions: [],
            SummaryText: string.Empty
        );

        var dto = new GroupDetailDto(
            Id: FixedId,
            Code: "B1-01",
            Name: "Английский B1",
            Description: "Группа для уровня B1",
            LevelId: FixedLevelId,
            LevelCode: "B1",
            LevelName: "B1 — Средний",
            LevelTone: LevelTone.Blue,
            CourseId: FixedCourseId,
            CourseCode: "EN-GEN-B1",
            CourseName: "Английский Общий B1",
            Teacher: new TeacherDto(FixedTeacherId, "Иванова Мария", "Преподаватель", null),
            RoomId: null,
            RoomLabel: null,
            Format: GroupFormat.Online,
            Platform: OnlinePlatform.Zoom,
            Schedule: schedule,
            UpcomingLessons: [],
            Capacity: 12,
            MemberCount: 8,
            Status: GroupStatus.Active,
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30)
        );

        return Verify(dto);
    }

    [Test]
    public Task GivenGroupDetailDtoWithFullSchedule_WhenSerialized_ThenSnapshotMatchesContract()
    {
        var schedule = new ScheduleDetailDto(
            Id: FixedScheduleId,
            Recurrence: "Weekly",
            BiweeklyParity: null,
            LessonDurationMinutes: 90,
            StartDate: new DateOnly(2025, 9, 1),
            EndMode: "Date",
            EndDate: new DateOnly(2026, 6, 30),
            LessonCount: null,
            SkipHolidays: false,
            Slots: [new ScheduleSlotDto(1, 1080), new ScheduleSlotDto(3, 1080)],
            Exceptions: [new ScheduleExceptionDto(new DateOnly(2025, 11, 4), "Праздник")],
            SummaryText: "Пн / Ср · 18:00–19:30"
        );

        IReadOnlyList<UpcomingLessonDto> upcomingLessons =
        [
            new(new DateOnly(2025, 9, 1), new TimeOnly(18, 0), new TimeOnly(19, 30)),
            new(new DateOnly(2025, 9, 3), new TimeOnly(18, 0), new TimeOnly(19, 30)),
            new(new DateOnly(2025, 9, 8), new TimeOnly(18, 0), new TimeOnly(19, 30)),
        ];

        var dto = new GroupDetailDto(
            Id: FixedId,
            Code: "B1-01",
            Name: "Английский B1",
            Description: "Группа для уровня B1",
            LevelId: FixedLevelId,
            LevelCode: "B1",
            LevelName: "B1 — Средний",
            LevelTone: LevelTone.Blue,
            CourseId: FixedCourseId,
            CourseCode: "EN-GEN-B1",
            CourseName: "Английский Общий B1",
            Teacher: new TeacherDto(FixedTeacherId, "Иванова Мария", "Преподаватель", null),
            RoomId: null,
            RoomLabel: null,
            Format: GroupFormat.Online,
            Platform: OnlinePlatform.Zoom,
            Schedule: schedule,
            UpcomingLessons: upcomingLessons,
            Capacity: 12,
            MemberCount: 8,
            Status: GroupStatus.Active,
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30)
        );

        return Verify(dto);
    }
}
