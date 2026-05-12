using Edvantix.Curriculum.Domain.Enums;

namespace Edvantix.Curriculum.Features.Courses;

/// <summary>DTO элемента списка курсов.</summary>
public sealed record CourseDto(
    Guid Id,
    string Code,
    string Name,
    CourseSubject Subject,
    string Level,
    short DurationWeeks,
    string? CoverInitials,
    CourseStatus Status,
    int TotalLessons
);

/// <summary>DTO детальной страницы курса (с модулями и уроками).</summary>
public sealed record CourseDetailDto(
    Guid Id,
    string Code,
    string Name,
    CourseSubject Subject,
    string Level,
    short DurationWeeks,
    string? Description,
    string? CoverInitials,
    CourseStatus Status,
    Guid OwnerMemberId,
    IReadOnlyList<CourseGoalDto> Goals,
    IReadOnlyList<ModuleDetailDto> Modules
);

/// <summary>DTO цели курса.</summary>
public sealed record CourseGoalDto(Guid Id, short Position, string Text);

/// <summary>DTO модуля с уроками (для детального просмотра курса).</summary>
public sealed record ModuleDetailDto(
    Guid Id,
    short Position,
    string Name,
    string? Summary,
    short Weeks,
    IReadOnlyList<LessonDto> Lessons
);

/// <summary>DTO урока.</summary>
public sealed record LessonDto(
    Guid Id,
    short Position,
    string Title,
    LessonType Type,
    LessonStatus Status,
    short Minutes
);

/// <summary>Лёгкий DTO курса для выпадающего списка (Group Create).</summary>
public sealed record CourseOptionDto(
    Guid Id,
    string Code,
    string Name,
    string Level,
    CourseSubject Subject
);
