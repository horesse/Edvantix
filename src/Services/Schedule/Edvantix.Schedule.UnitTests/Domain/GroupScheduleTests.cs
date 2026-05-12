namespace Edvantix.Schedule.UnitTests.Domain;

public sealed class GroupScheduleTests
{
    private static readonly Guid ValidGroupId = Guid.CreateVersion7();
    private static readonly Guid ValidOrganizationId = Guid.CreateVersion7();
    private static readonly DateOnly StartDate = new(2026, 1, 5);

    [Test]
    public void GivenValidData_WhenCreatingSchedule_ThenShouldInitializeProperties()
    {
        var schedule = CreateSchedule();

        schedule.GroupId.ShouldBe(ValidGroupId);
        schedule.OrganizationId.ShouldBe(ValidOrganizationId);
        schedule.Recurrence.ShouldBe(RecurrenceType.Weekly);
        schedule.LessonDurationMinutes.ShouldBe((short)60);
        schedule.StartDate.ShouldBe(StartDate);
        schedule.EndMode.ShouldBe(EndMode.Date);
        schedule.EndDate.ShouldBe(StartDate.AddMonths(1));
        schedule.SkipHolidays.ShouldBeFalse();
        schedule.NotifyStudents.ShouldBeTrue();
        schedule.Slots.ShouldBeEmpty();
        schedule.Exceptions.ShouldBeEmpty();
    }

    [Test]
    public void GivenEmptyGroupId_WhenCreatingSchedule_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new GroupSchedule(
                Guid.Empty,
                ValidOrganizationId,
                RecurrenceType.Weekly,
                60,
                StartDate,
                EndMode.Date,
                StartDate.AddDays(1),
                null,
                null,
                false,
                false
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingSchedule_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new GroupSchedule(
                ValidGroupId,
                Guid.Empty,
                RecurrenceType.Weekly,
                60,
                StartDate,
                EndMode.Date,
                StartDate.AddDays(1),
                null,
                null,
                false,
                false
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-15)]
    public void GivenInvalidDuration_WhenCreatingSchedule_ThenShouldThrowOutOfRange(short duration)
    {
        var act = () =>
            new GroupSchedule(
                ValidGroupId,
                ValidOrganizationId,
                RecurrenceType.Weekly,
                duration,
                StartDate,
                EndMode.Date,
                StartDate.AddDays(1),
                null,
                null,
                false,
                false
            );

        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Test]
    public void GivenDateEndModeWithoutEndDate_WhenCreatingSchedule_ThenShouldThrow()
    {
        var act = () =>
            new GroupSchedule(
                ValidGroupId,
                ValidOrganizationId,
                RecurrenceType.Weekly,
                60,
                StartDate,
                EndMode.Date,
                null,
                null,
                null,
                false,
                false
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenDateEndModeWithEndDateBeforeStart_WhenCreatingSchedule_ThenShouldThrow()
    {
        var act = () =>
            new GroupSchedule(
                ValidGroupId,
                ValidOrganizationId,
                RecurrenceType.Weekly,
                60,
                StartDate,
                EndMode.Date,
                StartDate,
                null,
                null,
                false,
                false
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenCountEndModeWithoutLessonCount_WhenCreatingSchedule_ThenShouldThrow()
    {
        var act = () =>
            new GroupSchedule(
                ValidGroupId,
                ValidOrganizationId,
                RecurrenceType.Weekly,
                60,
                StartDate,
                EndMode.Count,
                null,
                null,
                null,
                false,
                false
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenBiweeklyRecurrenceWithoutParity_WhenCreatingSchedule_ThenShouldThrow()
    {
        var act = () =>
            new GroupSchedule(
                ValidGroupId,
                ValidOrganizationId,
                RecurrenceType.Biweekly,
                60,
                StartDate,
                EndMode.Count,
                null,
                3,
                null,
                false,
                false
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNewSettings_WhenUpdating_ThenShouldUpdateProperties()
    {
        var schedule = CreateSchedule();

        schedule.UpdateSettings(
            RecurrenceType.Biweekly,
            90,
            EndMode.Count,
            null,
            10,
            1,
            skipHolidays: true,
            notifyStudents: false
        );

        schedule.Recurrence.ShouldBe(RecurrenceType.Biweekly);
        schedule.LessonDurationMinutes.ShouldBe((short)90);
        schedule.EndMode.ShouldBe(EndMode.Count);
        schedule.LessonCount.ShouldBe((short)10);
        schedule.BiweeklyParity.ShouldBe(1);
        schedule.SkipHolidays.ShouldBeTrue();
        schedule.NotifyStudents.ShouldBeFalse();
    }

    [Test]
    public void GivenSlots_WhenReplacing_ThenShouldUseNewSlotsOnly()
    {
        var schedule = CreateSchedule();

        schedule.ReplaceSlots([(1, 600), (3, 720)]);
        schedule.ReplaceSlots([(5, 900)]);

        schedule.Slots.ShouldHaveSingleItem();
        schedule.Slots[0].Weekday.ShouldBe(5);
        schedule.Slots[0].StartMinutes.ShouldBe(900);
    }

    [Test]
    public void GivenNewException_WhenAdding_ThenShouldReturnExceptionAndStoreIt()
    {
        var schedule = CreateSchedule();
        var date = StartDate.AddDays(7);

        var exception = schedule.AddException(date, "  holiday  ");

        exception.ScheduleId.ShouldBe(schedule.Id);
        exception.ExceptionDate.ShouldBe(date);
        exception.Reason.ShouldBe("holiday");
        schedule.Exceptions.ShouldHaveSingleItem();
    }

    [Test]
    public void GivenDuplicateExceptionDate_WhenAdding_ThenShouldThrow()
    {
        var schedule = CreateSchedule();
        var date = StartDate.AddDays(7);
        schedule.AddException(date);

        var act = () => schedule.AddException(date);

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenExistingException_WhenRemovingById_ThenShouldRemoveIt()
    {
        var schedule = CreateSchedule();
        var exception = schedule.AddException(StartDate.AddDays(7));

        schedule.RemoveException(exception.Id);

        schedule.Exceptions.ShouldBeEmpty();
    }

    [Test]
    public void GivenEmptyScheduleRequest_WhenCreatingEmpty_ThenShouldHaveDefaultSettings()
    {
        var schedule = GroupSchedule.CreateEmpty(ValidGroupId, ValidOrganizationId, StartDate);

        schedule.Recurrence.ShouldBe(RecurrenceType.Weekly);
        schedule.LessonDurationMinutes.ShouldBe((short)60);
        schedule.EndMode.ShouldBe(EndMode.Date);
        schedule.EndDate.ShouldBe(StartDate.AddYears(1));
        schedule.Slots.ShouldBeEmpty();
    }

    [Test]
    public void GivenWeeklyScheduleWithSlots_WhenMaterializing_ThenShouldCreateOccurrences()
    {
        var schedule = CreateSchedule(endMode: EndMode.Count, endDate: null, lessonCount: 3);
        schedule.ReplaceSlots([(1, 600), (3, 720)]);

        var occurrences = schedule.Materialize([]);

        occurrences.Count.ShouldBe(3);
        occurrences
            .Select(o => o.LessonDate)
            .ShouldBe([
                new DateOnly(2026, 1, 5),
                new DateOnly(2026, 1, 7),
                new DateOnly(2026, 1, 12),
            ]);
        occurrences.All(o => o.ScheduleId == schedule.Id).ShouldBeTrue();
        occurrences.All(o => o.GroupId == ValidGroupId).ShouldBeTrue();
    }

    [Test]
    public void GivenExceptionAndHoliday_WhenMaterializing_ThenShouldSkipThoseDates()
    {
        var schedule = CreateSchedule(
            endMode: EndMode.Count,
            endDate: null,
            lessonCount: 2,
            skipHolidays: true
        );
        schedule.ReplaceSlots([(1, 600)]);
        schedule.AddException(new DateOnly(2026, 1, 5));
        var holiday = new Holiday("blr", new DateOnly(2026, 1, 12), "Holiday");

        var occurrences = schedule.Materialize([holiday]);

        occurrences
            .Select(o => o.LessonDate)
            .ShouldBe([new DateOnly(2026, 1, 19), new DateOnly(2026, 1, 26)]);
    }

    private static GroupSchedule CreateSchedule(
        EndMode endMode = EndMode.Date,
        DateOnly? endDate = null,
        short? lessonCount = null,
        bool skipHolidays = false
    ) =>
        new(
            ValidGroupId,
            ValidOrganizationId,
            RecurrenceType.Weekly,
            60,
            StartDate,
            endMode,
            endDate ?? StartDate.AddMonths(1),
            lessonCount,
            null,
            skipHolidays,
            notifyStudents: true
        );
}
