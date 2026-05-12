using Edvantix.Schedule.Domain.AggregatesModel.HolidayAggregate;
using Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;
using Edvantix.Schedule.Domain.Enums;
using Edvantix.SharedKernel.Helpers;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;

/// <summary>
/// Расписание учебной группы — корневой агрегат.
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item>На одну группу — одно расписание (уникальность по <see cref="GroupId"/>).</item>
///   <item><see cref="EndMode.Date"/> требует указания <see cref="EndDate"/>.</item>
///   <item><see cref="EndMode.Count"/> требует указания <see cref="LessonCount"/>.</item>
///   <item><see cref="RecurrenceType.Biweekly"/> требует указания <see cref="BiweeklyParity"/>.</item>
/// </list>
/// </summary>
public sealed class GroupSchedule() : Entity, IAggregateRoot, ITenanted
{
    private readonly List<ScheduleSlot> _slots = [];
    private readonly List<ScheduleException> _exceptions = [];

    /// <param name="groupId">Идентификатор группы (логическая FK на Organizational).</param>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="recurrence">Тип рекуррентности.</param>
    /// <param name="lessonDurationMinutes">Длительность занятия в минутах.</param>
    /// <param name="startDate">Дата начала расписания.</param>
    /// <param name="endMode">Способ определения конца расписания.</param>
    /// <param name="endDate">Дата окончания (при <see cref="EndMode.Date"/>).</param>
    /// <param name="lessonCount">Число занятий (при <see cref="EndMode.Count"/>).</param>
    /// <param name="biweeklyParity">Чётность недель (0 или 1; при <see cref="RecurrenceType.Biweekly"/>).</param>
    /// <param name="skipHolidays">Автопропуск государственных праздников.</param>
    /// <param name="notifyStudents">Отправлять уведомления студентам при изменениях.</param>
    public GroupSchedule(
        Guid groupId,
        Guid organizationId,
        RecurrenceType recurrence,
        short lessonDurationMinutes,
        DateOnly startDate,
        EndMode endMode,
        DateOnly? endDate,
        short? lessonCount,
        int? biweeklyParity,
        bool skipHolidays,
        bool notifyStudents
    )
        : this()
    {
        if (groupId == Guid.Empty)
            throw new ArgumentException("Идентификатор группы не может быть пустым.", nameof(groupId));

        if (organizationId == Guid.Empty)
            throw new ArgumentException("Идентификатор организации не может быть пустым.", nameof(organizationId));

        if (lessonDurationMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(lessonDurationMinutes), "Длительность занятия должна быть больше нуля.");

        ValidateEndMode(endMode, endDate, lessonCount, startDate);
        ValidateBiweeklyParity(recurrence, biweeklyParity);

        Id = Guid.CreateVersion7();
        GroupId = groupId;
        OrganizationId = organizationId;
        Recurrence = recurrence;
        LessonDurationMinutes = lessonDurationMinutes;
        StartDate = startDate;
        EndMode = endMode;
        EndDate = endDate;
        LessonCount = lessonCount;
        BiweeklyParity = biweeklyParity;
        SkipHolidays = skipHolidays;
        NotifyStudents = notifyStudents;
        CreatedAt = DateTimeHelper.UtcNow();
        UpdatedAt = CreatedAt;
    }

    /// <summary>Идентификатор группы (логическая FK на Organizational.groups).</summary>
    public Guid GroupId { get; private set; }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Тип рекуррентности.</summary>
    public RecurrenceType Recurrence { get; private set; }

    /// <summary>
    /// Чётность недель (0/1) при <see cref="RecurrenceType.Biweekly"/>.
    /// Для остальных типов — <c>null</c>.
    /// </summary>
    public int? BiweeklyParity { get; private set; }

    /// <summary>Длительность занятия в минутах.</summary>
    public short LessonDurationMinutes { get; private set; }

    /// <summary>Дата начала расписания.</summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>Способ определения конца расписания.</summary>
    public EndMode EndMode { get; private set; }

    /// <summary>Дата окончания (при <see cref="EndMode.Date"/>).</summary>
    public DateOnly? EndDate { get; private set; }

    /// <summary>Количество занятий (при <see cref="EndMode.Count"/>).</summary>
    public short? LessonCount { get; private set; }

    /// <summary>Автоматически пропускать государственные праздники.</summary>
    public bool SkipHolidays { get; private set; }

    /// <summary>Уведомлять студентов при изменениях.</summary>
    public bool NotifyStudents { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Недельные слоты расписания.</summary>
    public IReadOnlyList<ScheduleSlot> Slots => _slots;

    /// <summary>Ручные исключения (пропуски) расписания.</summary>
    public IReadOnlyList<ScheduleException> Exceptions => _exceptions;

    /// <summary>
    /// Обновляет параметры расписания и сбрасывает слоты.
    /// После вызова необходимо пересоздать <c>LessonOccurrence</c> через <see cref="Materialize"/>.
    /// </summary>
    public void UpdateSettings(
        RecurrenceType recurrence,
        short lessonDurationMinutes,
        EndMode endMode,
        DateOnly? endDate,
        short? lessonCount,
        int? biweeklyParity,
        bool skipHolidays,
        bool notifyStudents
    )
    {
        if (lessonDurationMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(lessonDurationMinutes), "Длительность занятия должна быть больше нуля.");

        ValidateEndMode(endMode, endDate, lessonCount, StartDate);
        ValidateBiweeklyParity(recurrence, biweeklyParity);

        Recurrence = recurrence;
        LessonDurationMinutes = lessonDurationMinutes;
        EndMode = endMode;
        EndDate = endDate;
        LessonCount = lessonCount;
        BiweeklyParity = biweeklyParity;
        SkipHolidays = skipHolidays;
        NotifyStudents = notifyStudents;
        UpdatedAt = DateTimeHelper.UtcNow();
    }

    /// <summary>Заменяет набор слотов новым.</summary>
    public void ReplaceSlots(IEnumerable<(int Weekday, int StartMinutes)> slots)
    {
        _slots.Clear();
        foreach (var (weekday, startMinutes) in slots)
            _slots.Add(new ScheduleSlot(Id, weekday, startMinutes));

        UpdatedAt = DateTimeHelper.UtcNow();
    }

    /// <summary>Добавляет исключение (пропуск) на конкретную дату.</summary>
    /// <returns>Созданное исключение.</returns>
    /// <exception cref="InvalidOperationException">Исключение на эту дату уже существует.</exception>
    public ScheduleException AddException(DateOnly date, string? reason = null)
    {
        if (_exceptions.Any(e => e.ExceptionDate == date))
            throw new InvalidOperationException($"Исключение на {date:yyyy-MM-dd} уже добавлено.");

        var exception = new ScheduleException(Id, date, reason);
        _exceptions.Add(exception);
        UpdatedAt = DateTimeHelper.UtcNow();
        return exception;
    }

    /// <summary>Удаляет исключение по дате.</summary>
    public void RemoveException(DateOnly date)
    {
        var exception = _exceptions.FirstOrDefault(e => e.ExceptionDate == date);
        if (exception is not null)
        {
            _exceptions.Remove(exception);
            UpdatedAt = DateTimeHelper.UtcNow();
        }
    }

    /// <summary>Удаляет исключение по идентификатору.</summary>
    public void RemoveException(Guid exceptionId)
    {
        var exception = _exceptions.FirstOrDefault(e => e.Id == exceptionId);
        if (exception is not null)
        {
            _exceptions.Remove(exception);
            UpdatedAt = DateTimeHelper.UtcNow();
        }
    }

    /// <summary>
    /// Создаёт пустое расписание без слотов. Используется при автоматическом создании при появлении группы.
    /// </summary>
    public static GroupSchedule CreateEmpty(Guid groupId, Guid organizationId, DateOnly startDate) =>
        new(
            groupId,
            organizationId,
            RecurrenceType.Weekly,
            lessonDurationMinutes: 60,
            startDate,
            EndMode.Date,
            endDate: startDate.AddYears(1),
            lessonCount: null,
            biweeklyParity: null,
            skipHolidays: false,
            notifyStudents: false
        );

    /// <summary>
    /// Материализует список занятий на основе слотов, исключений и праздников.
    /// Результат используется для сохранения <c>LessonOccurrence</c> записей.
    /// </summary>
    /// <param name="holidays">Праздники страны для автопропуска (при <see cref="SkipHolidays"/> = true).</param>
    public IReadOnlyList<LessonOccurrence> Materialize(IReadOnlyList<Holiday> holidays)
    {
        if (_slots.Count == 0)
            return [];

        var holidayDates = SkipHolidays
            ? holidays.Select(h => h.Date).ToHashSet()
            : [];

        var exceptionDates = _exceptions.Select(e => e.ExceptionDate).ToHashSet();

        var occurrences = new List<LessonOccurrence>();
        var current = StartDate;
        short count = 0;

        var limit = EndMode switch
        {
            EndMode.Count => LessonCount ?? short.MaxValue,
            EndMode.Date => short.MaxValue,
            _ => short.MaxValue,
        };

        // Определяем дату отсечки
        var endCutoff = EndMode == EndMode.Date && EndDate.HasValue
            ? EndDate.Value
            : DateOnly.MaxValue;

        while (current <= endCutoff && count < limit)
        {
            // Биеженедельная проверка
            if (Recurrence == RecurrenceType.Biweekly && BiweeklyParity.HasValue)
            {
                var weekNumber = GetIso8601WeekNumber(current);
                if (weekNumber % 2 != BiweeklyParity.Value)
                {
                    current = current.AddDays(1);
                    continue;
                }
            }

            foreach (var slot in _slots.OrderBy(s => s.Weekday).ThenBy(s => s.StartMinutes))
            {
                if ((int)current.DayOfWeek != slot.Weekday)
                    continue;

                if (exceptionDates.Contains(current) || holidayDates.Contains(current))
                    continue;

                occurrences.Add(
                    new LessonOccurrence(
                        Id,
                        GroupId,
                        current,
                        slot.StartMinutes,
                        LessonDurationMinutes
                    )
                );

                count++;
                if (count >= limit)
                    break;
            }

            current = current.AddDays(1);
        }

        return occurrences;
    }

    private static void ValidateEndMode(
        EndMode endMode,
        DateOnly? endDate,
        short? lessonCount,
        DateOnly startDate
    )
    {
        if (endMode == EndMode.Date)
        {
            if (!endDate.HasValue)
                throw new ArgumentException("При EndMode.Date необходимо указать дату окончания.", nameof(endDate));

            if (endDate.Value <= startDate)
                throw new ArgumentException("Дата окончания должна быть позже даты начала.", nameof(endDate));
        }

        if (endMode == EndMode.Count)
        {
            if (!lessonCount.HasValue || lessonCount.Value <= 0)
                throw new ArgumentException("При EndMode.Count необходимо указать положительное число занятий.", nameof(lessonCount));
        }
    }

    private static void ValidateBiweeklyParity(RecurrenceType recurrence, int? biweeklyParity)
    {
        if (recurrence == RecurrenceType.Biweekly && biweeklyParity is not (0 or 1))
            throw new ArgumentException(
                "При Biweekly рекуррентности необходимо указать чётность недели (0 или 1).",
                nameof(biweeklyParity)
            );
    }

    // ISO 8601: неделя 1 — первая неделя с четвергом текущего года.
    private static int GetIso8601WeekNumber(DateOnly date)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);
        var day = (int)System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dateTime);
        dateTime = dateTime.AddDays(4 - (day == 0 ? 7 : day));
        return (dateTime.DayOfYear - 1) / 7 + 1;
    }
}
