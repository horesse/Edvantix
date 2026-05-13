using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Features.OrganizationMembers;

namespace Edvantix.Organizational.Features.Groups;

/// <summary>DTO для строки в постраничном списке групп (Groups.html).</summary>
public sealed record GroupListItemDto(
    [property: Description("Идентификатор группы")] Guid Id,
    [property: Description("Уникальный код группы")] string Code,
    [property: Description("Название группы")] string Name,
    [property: Description("Уровень")] GroupLevel Level,
    [property: Description("Формат занятий")] GroupFormat Format,
    [property: Description("Статус")] GroupStatus Status,
    [property: Description("Максимальная вместимость")] int Capacity,
    [property: Description("Количество активных участников")] int MemberCount,
    [property: Description("Дата начала")] DateOnly StartDate,
    [property: Description("Дата окончания")] DateOnly EndDate,
    [property: Description("Преподаватель группы")] TeacherDto Teacher,
    [property: Description("Идентификатор кабинета")] Guid? RoomId,
    [property: Description("Метка кабинета")] string? RoomLabel,
    [property: Description("Идентификатор курса")] Guid CourseId
);

/// <summary>DTO детальной карточки группы (Group Create preview / Group Edit).</summary>
public sealed record GroupDetailDto(
    [property: Description("Идентификатор группы")] Guid Id,
    [property: Description("Уникальный код группы")] string Code,
    [property: Description("Название группы")] string Name,
    [property: Description("Описание группы")] string Description,
    [property: Description("Уровень")] GroupLevel Level,
    [property: Description("Формат занятий")] GroupFormat Format,
    [property: Description("Статус")] GroupStatus Status,
    [property: Description("Максимальная вместимость")] int Capacity,
    [property: Description("Количество активных участников")] int MemberCount,
    [property: Description("Дата начала")] DateOnly StartDate,
    [property: Description("Дата окончания")] DateOnly EndDate,
    [property: Description("Идентификатор курса")] Guid CourseId,
    [property: Description("Преподаватель группы")] TeacherDto Teacher,
    [property: Description("Идентификатор кабинета")] Guid? RoomId,
    [property: Description("Метка кабинета")] string? RoomLabel,
    [property: Description("Онлайн-платформа")] OnlinePlatform? Platform
);

/// <summary>KPI-статистика групп организации.</summary>
public sealed record GroupStatsDto(
    [property: Description("Всего групп")] int Total,
    [property: Description("Активные группы")] int Active,
    [property: Description("Группы на наборе")] int Recruiting,
    [property: Description("Группы на паузе")] int Paused,
    [property: Description("Завершённые группы")] int Finished,
    [property: Description("Архивированные группы")] int Archived
);
