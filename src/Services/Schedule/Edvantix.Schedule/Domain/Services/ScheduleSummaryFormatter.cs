namespace Edvantix.Schedule.Domain.Services;

/// <summary>
/// Форматирует слоты расписания в человекочитаемую строку на русском языке.
/// <example>
/// <code>
/// // "Пн / Ср · 18:00–19:30"
/// ScheduleSummaryFormatter.Format([(1, 1080), (3, 1080)], 90);
///
/// // "Пн 18:00–19:30 · Сб 10:00–13:00"
/// ScheduleSummaryFormatter.Format([(1, 1080), (6, 600)], 90);
/// </code>
/// </example>
/// </summary>
public static class ScheduleSummaryFormatter
{
    // DayOfWeek mapping: 0=Вс, 1=Пн, 2=Вт, 3=Ср, 4=Чт, 5=Пт, 6=Сб
    private static readonly string[] WeekdayNames = ["Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб"];

    /// <summary>
    /// Строит строку расписания из слотов.
    /// </summary>
    /// <param name="slots">Последовательность (Weekday 0–6, StartMinutes 0–1439).</param>
    /// <param name="durationMinutes">Длительность занятия в минутах.</param>
    /// <returns>
    /// Строка вида "Пн / Ср · 18:00–19:30", или пустая строка если слотов нет.
    /// </returns>
    public static string Format(IEnumerable<(int Weekday, int StartMinutes)> slots, int durationMinutes)
    {
        var sorted = slots.OrderBy(s => s.Weekday).ThenBy(s => s.StartMinutes).ToList();

        if (sorted.Count == 0)
            return string.Empty;

        // Группируем по StartMinutes: одно окно = один сегмент строки.
        // Порядок сегментов — по минимальному Weekday в группе (т.е. по первому дню недели в
        // исходной сортировке), чтобы "Пн / Ср · 18:00–19:30 · Сб 10:00–11:30" давало Mon раньше Sat.
        var segments = sorted
            .GroupBy(s => s.StartMinutes)
            .OrderBy(g => g.Min(s => s.Weekday))
            .ThenBy(g => g.Key)
            .Select(g => FormatSegment(g.Select(s => s.Weekday).ToList(), g.Key, durationMinutes));

        return string.Join(" · ", segments);
    }

    private static string FormatSegment(List<int> weekdays, int startMinutes, int durationMinutes)
    {
        var timeRange = $"{FormatTime(startMinutes)}–{FormatTime(startMinutes + durationMinutes)}";

        // Единственный день: "Пн 18:00–19:30"
        if (weekdays.Count == 1)
            return $"{WeekdayNames[weekdays[0]]} {timeRange}";

        // Несколько дней одного окна: "Пн / Ср · 18:00–19:30"
        var dayPart = string.Join(" / ", weekdays.Select(d => WeekdayNames[d]));
        return $"{dayPart} · {timeRange}";
    }

    private static string FormatTime(int totalMinutes)
    {
        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;
        return $"{hours:D2}:{minutes:D2}";
    }
}
