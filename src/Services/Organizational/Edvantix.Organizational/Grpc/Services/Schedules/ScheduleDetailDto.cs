namespace Edvantix.Organizational.Grpc.Services.Schedules;

/// <summary>Полные детали расписания группы, получаемые через gRPC из Schedule-сервиса.</summary>
public sealed record ScheduleDetailDto(
    [property: Description("Идентификатор расписания")] Guid Id,
    [property: Description("Тип рекуррентности: Weekly / Biweekly / Custom")] string Recurrence,
    [property: Description("Чётность недели (0 или 1) для Biweekly; null для остальных")]
        int? BiweeklyParity,
    [property: Description("Длительность занятия в минутах")] short LessonDurationMinutes,
    [property: Description("Дата начала расписания")] DateOnly StartDate,
    [property: Description("Способ определения конца: Date / Count")] string EndMode,
    [property: Description("Дата окончания (при EndMode=Date)")] DateOnly? EndDate,
    [property: Description("Число занятий (при EndMode=Count)")] short? LessonCount,
    [property: Description("Автопропуск государственных праздников")] bool SkipHolidays,
    [property: Description("Недельные временны́е слоты")] IReadOnlyList<ScheduleSlotDto> Slots,
    [property: Description("Ручные исключения (пропуски)")]
        IReadOnlyList<ScheduleExceptionDto> Exceptions,
    [property: Description("Человекочитаемая сводка, напр. Пн / Ср · 18:00–19:30")]
        string SummaryText
);

/// <summary>Временно́й слот в недельной сетке расписания.</summary>
public sealed record ScheduleSlotDto(
    [property: Description("День недели (0 = Вс … 6 = Сб)")] int Weekday,
    [property: Description("Начало занятия в минутах от полуночи")] int StartMinutes
);

/// <summary>Исключение (пропуск) на конкретную дату.</summary>
public sealed record ScheduleExceptionDto(
    [property: Description("Дата пропуска")] DateOnly Date,
    [property: Description("Причина пропуска; null если не указана")] string? Reason
);
