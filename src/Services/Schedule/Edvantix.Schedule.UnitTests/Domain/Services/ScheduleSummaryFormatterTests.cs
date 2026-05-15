using Edvantix.Schedule.Domain.Services;

namespace Edvantix.Schedule.UnitTests.Domain.Services;

public sealed class ScheduleSummaryFormatterTests
{
    // ── single slot ───────────────────────────────────────────────────────────

    [Test]
    public void GivenSingleSlot_WhenFormat_ThenReturnsDayAndTimeRange()
    {
        // Monday 18:00, 90 min → 19:30
        var result = ScheduleSummaryFormatter.Format([(1, 1080)], 90);

        result.ShouldBe("Пн 18:00–19:30");
    }

    // ── multiple days, same window (primary scenario) ─────────────────────────

    [Test]
    public void GivenWeeklyMonWed18_WhenSummarize_ThenReturnsRuRuFormat()
    {
        // Monday=1, Wednesday=3, StartMinutes=1080 (18:00), Duration=90 → 19:30
        var result = ScheduleSummaryFormatter.Format([(1, 1080), (3, 1080)], 90);

        result.ShouldBe("Пн / Ср · 18:00–19:30");
    }

    [Test]
    public void GivenThreeDaysSameWindow_WhenFormat_ThenJoinsAllWithSlash()
    {
        var result = ScheduleSummaryFormatter.Format([(1, 1080), (3, 1080), (5, 1080)], 90);

        result.ShouldBe("Пн / Ср / Пт · 18:00–19:30");
    }

    // ── different windows ─────────────────────────────────────────────────────

    [Test]
    public void GivenDifferentSlotWindows_WhenSummarize_ThenReturnsMultipleSegments()
    {
        // Mon 18:00–19:30, Sat 10:00–11:30
        var result = ScheduleSummaryFormatter.Format([(1, 1080), (6, 600)], 90);

        result.ShouldBe("Пн 18:00–19:30 · Сб 10:00–11:30");
    }

    [Test]
    public void GivenMultiDayWindowAndSingleDayWindow_WhenFormat_ThenReturnsBothSegments()
    {
        // Mon+Wed 18:00–19:30, Sat 10:00–11:30
        var result = ScheduleSummaryFormatter.Format([(1, 1080), (3, 1080), (6, 600)], 90);

        result.ShouldBe("Пн / Ср · 18:00–19:30 · Сб 10:00–11:30");
    }

    // ── empty slots ───────────────────────────────────────────────────────────

    [Test]
    public void GivenEmptySlots_WhenSummarize_ThenReturnsEmpty()
    {
        var result = ScheduleSummaryFormatter.Format([], 90);

        result.ShouldBeEmpty();
    }

    // ── sorting ───────────────────────────────────────────────────────────────

    [Test]
    public void GivenUnsortedSlots_WhenFormat_ThenSortsByWeekdayThenStartMinutes()
    {
        // Wed then Mon in input — result must sort Mon first
        var result = ScheduleSummaryFormatter.Format([(3, 1080), (1, 1080)], 60);

        result.ShouldBe("Пн / Ср · 18:00–19:00");
    }

    [Test]
    public void GivenSundaySlot_WhenFormat_ThenMapsToВс()
    {
        var result = ScheduleSummaryFormatter.Format([(0, 600)], 60);

        result.ShouldBe("Вс 10:00–11:00");
    }

    [Test]
    public void GivenSaturdaySlot_WhenFormat_ThenMapsToСб()
    {
        var result = ScheduleSummaryFormatter.Format([(6, 0)], 60);

        result.ShouldBe("Сб 00:00–01:00");
    }

    // ── time display ──────────────────────────────────────────────────────────

    [Test]
    public void GivenMidnightSlot_WhenFormat_ThenFormatsWithLeadingZeros()
    {
        var result = ScheduleSummaryFormatter.Format([(1, 0)], 45);

        result.ShouldBe("Пн 00:00–00:45");
    }

    [Test]
    public void GivenSlotAt9_30_WhenFormatWith60Min_ThenEndTimeIs10_30()
    {
        var result = ScheduleSummaryFormatter.Format([(1, 570)], 60);

        result.ShouldBe("Пн 09:30–10:30");
    }

    // ── contract: ru-RU locale ────────────────────────────────────────────────

    [Test]
    public void GivenRuRuWeekdays_WhenFormat_ThenTextContainsOnlyCyrillicAbbreviations()
    {
        var result = ScheduleSummaryFormatter.Format(
            [(1, 1080), (2, 1080), (3, 1080), (4, 1080), (5, 1080)],
            90
        );

        result.ShouldBe("Пн / Вт / Ср / Чт / Пт · 18:00–19:30");

        result.ShouldNotContain("Mon");
        result.ShouldNotContain("Wed");
    }
}
