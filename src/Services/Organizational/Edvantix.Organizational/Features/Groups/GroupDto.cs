using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Features.OrganizationMembers;
using Edvantix.Organizational.Grpc.Services.Schedules;

namespace Edvantix.Organizational.Features.Groups;

/// <summary>DTO для строки в постраничном списке групп (Groups.html).</summary>
public sealed record GroupListItemDto(
    [property: Description("Идентификатор группы")] Guid Id,
    [property: Description("Уникальный код группы")] string Code,
    [property: Description("Название группы")] string Name,
    // Level — справочник из БД (navigation property)
    [property: Description("Идентификатор уровня")] Guid LevelId,
    [property: Description("Код уровня, напр. A1 или JR")] string LevelCode,
    [property: Description("Название уровня, напр. A1 — Начальный")] string LevelName,
    [property: Description("Цветовой тон уровня для UI-бейджа")] LevelTone LevelTone,
    // Course — обогащается через gRPC к Curriculum-сервису
    [property: Description("Идентификатор курса")] Guid CourseId,
    [property: Description("Код курса, напр. EN-GEN-B1")] string CourseCode,
    [property: Description("Название курса")] string CourseName,
    // Teacher — обогащается через gRPC к Profile-сервису
    [property: Description("Преподаватель группы")] TeacherDto Teacher,
    // Room — обогащается из БД
    [property: Description("Идентификатор кабинета")] Guid? RoomId,
    [property: Description("Метка кабинета")] string? RoomLabel,
    // Format
    [property: Description("Формат занятий")] GroupFormat Format,
    [property: Description("Онлайн-платформа (только для Online-формата)")]
        OnlinePlatform? Platform,
    // Schedule summary — заполняется в Task 7 (пока null)
    [property: Description(
        "Сводка расписания, напр. Пн / Ср · 18:00–19:30"
    )] string? ScheduleSummary,
    // Members
    [property: Description("Максимальная вместимость")] int Capacity,
    [property: Description("Количество активных участников")] int MemberCount,
    // Lifecycle
    [property: Description("Статус")] GroupStatus Status,
    [property: Description("Дата начала")] DateOnly StartDate,
    [property: Description("Дата окончания")] DateOnly EndDate
);

/// <summary>DTO детальной карточки группы (Group Create preview / Group Edit).</summary>
public sealed record GroupDetailDto(
    [property: Description("Идентификатор группы")] Guid Id,
    [property: Description("Уникальный код группы")] string Code,
    [property: Description("Название группы")] string Name,
    [property: Description("Описание группы")] string Description,
    // Level — справочник из БД (navigation property)
    [property: Description("Идентификатор уровня")] Guid LevelId,
    [property: Description("Код уровня, напр. A1 или JR")] string LevelCode,
    [property: Description("Название уровня, напр. A1 — Начальный")] string LevelName,
    [property: Description("Цветовой тон уровня для UI-бейджа")] LevelTone LevelTone,
    // Course — обогащается через gRPC к Curriculum-сервису
    [property: Description("Идентификатор курса")] Guid CourseId,
    [property: Description("Код курса, напр. EN-GEN-B1")] string CourseCode,
    [property: Description("Название курса")] string CourseName,
    // Teacher — обогащается через gRPC к Profile-сервису
    [property: Description("Преподаватель группы")] TeacherDto Teacher,
    // Room — обогащается из БД
    [property: Description("Идентификатор кабинета")] Guid? RoomId,
    [property: Description("Метка кабинета")] string? RoomLabel,
    // Format
    [property: Description("Формат занятий")] GroupFormat Format,
    [property: Description("Онлайн-платформа (только для Online-формата)")]
        OnlinePlatform? Platform,
    // Schedule details — заполняется в Task 8
    [property: Description("Детали расписания")] ScheduleDetailDto? Schedule,
    [property: Description("Ближайшие занятия")] IReadOnlyList<UpcomingLessonDto> UpcomingLessons,
    // Members
    [property: Description("Максимальная вместимость")] int Capacity,
    [property: Description("Количество активных участников")] int MemberCount,
    // Lifecycle
    [property: Description("Статус")] GroupStatus Status,
    [property: Description("Дата начала")] DateOnly StartDate,
    [property: Description("Дата окончания")] DateOnly EndDate
);

/// <summary>KPI-статистика групп организации.</summary>
public sealed record GroupStatsDto(
    [property: Description("Всего групп")] int Total,
    [property: Description("Активные группы")] int Active,
    [property: Description("Группы на наборе")] int Recruiting,
    [property: Description("Группы на паузе")] int Paused,
    [property: Description("Завершённые группы")] int Finished,
    [property: Description("Архивированные группы")] int Archived,
    [property: Description("Сумма активных участников по группам со статусом Active")]
        int TotalActiveStudents,
    [property: Description("Сумма вместимости по всем НЕ-архивированным группам")]
        int TotalCapacity,
    [property: Description("Сумма активных участников по всем НЕ-архивированным группам")]
        int TotalFilledSeats,
    [property: Description("Процент заполненности (0 если TotalCapacity = 0)")] int FillRatePercent
);
