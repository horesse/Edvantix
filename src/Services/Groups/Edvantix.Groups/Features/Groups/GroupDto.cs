using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Groups.Grpc.Services.Schedules;

namespace Edvantix.Groups.Features.Groups;

/// <summary>DTO для строки в списке групп.</summary>
public sealed record GroupListItemDto(
    [property: Description("Идентификатор группы")] Guid Id,
    [property: Description("Код группы")] string Code,
    [property: Description("Наименование группы")] string Name,
    [property: Description("Статус группы")] GroupStatus Status,
    [property: Description("Формат занятий")] GroupFormat Format,
    [property: Description("Вместимость группы")] int Capacity,
    [property: Description("Идентификатор уровня")] Guid LevelId,
    [property: Description("Код уровня")] string LevelCode,
    [property: Description("Название уровня")] string LevelName,
    [property: Description("Дата начала")] DateOnly StartDate,
    [property: Description("Дата окончания")] DateOnly EndDate,
    [property: Description("Код курса из Curriculum-сервиса")] string? CourseCode,
    [property: Description("Название курса из Curriculum-сервиса")] string? CourseName
);

/// <summary>DTO детального представления группы.</summary>
public sealed record GroupDetailDto(
    [property: Description("Идентификатор группы")] Guid Id,
    [property: Description("Код группы")] string Code,
    [property: Description("Наименование группы")] string Name,
    [property: Description("Описание группы")] string Description,
    [property: Description("Статус группы")] GroupStatus Status,
    [property: Description("Формат занятий")] GroupFormat Format,
    [property: Description("Вместимость группы")] int Capacity,
    [property: Description("Идентификатор уровня")] Guid LevelId,
    [property: Description("Код уровня")] string LevelCode,
    [property: Description("Название уровня")] string LevelName,
    [property: Description("Идентификатор курса")] Guid CourseId,
    [property: Description("Код курса")] string? CourseCode,
    [property: Description("Название курса")] string? CourseName,
    [property: Description("Идентификатор преподавателя")] Guid TeacherMemberId,
    [property: Description("Информация о преподавателе")] TeacherDto Teacher,
    [property: Description("Идентификатор кабинета")] Guid? RoomId,
    [property: Description("Онлайн-платформа")] OnlinePlatform? Platform,
    [property: Description("Дата начала")] DateOnly StartDate,
    [property: Description("Дата окончания")] DateOnly EndDate,
    [property: Description("Расписание группы")] ScheduleDetailDto? Schedule,
    [property: Description("Ближайшие занятия")] IReadOnlyList<UpcomingLessonDto> UpcomingLessons
);

/// <summary>DTO статистики групп организации.</summary>
public sealed record GroupStatsDto(
    [property: Description("Всего групп")] int Total,
    [property: Description("Активных")] int Active,
    [property: Description("На наборе")] int Recruiting,
    [property: Description("На паузе")] int Paused,
    [property: Description("Завершённых")] int Finished,
    [property: Description("Архивных")] int Archived,
    [property: Description("Всего активных студентов в активных группах")] int TotalActiveStudents,
    [property: Description("Суммарная вместимость (без архивных)")] int TotalCapacity,
    [property: Description("Суммарно занятых мест (без архивных)")] int TotalFilledSeats,
    [property: Description("Процент заполнения (0–100)")] int FillRatePercent
);

/// <summary>DTO преподавателя группы.</summary>
public sealed record TeacherDto(
    [property: Description("Идентификатор участника организации")] Guid MemberId,
    [property: Description("Полное имя преподавателя")] string FullName,
    [property: Description("URL аватара")] string? AvatarUrl
);
