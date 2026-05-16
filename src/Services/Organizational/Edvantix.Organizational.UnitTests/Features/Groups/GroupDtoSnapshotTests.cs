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
}
