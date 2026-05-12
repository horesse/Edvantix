namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.UpdateSettings;

public sealed class UpdateGroupScheduleSettingsValidatorTests
{
    private readonly UpdateGroupScheduleSettingsValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveErrors()
    {
        var result = _validator.TestValidate(CreateValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyGroupId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(CreateValidCommand(groupId: Guid.Empty));

        result.ShouldHaveValidationErrorFor(c => c.GroupId);
    }

    [Test]
    public void GivenInvalidDuration_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(CreateValidCommand(duration: 0));

        result.ShouldHaveValidationErrorFor(c => c.LessonDurationMinutes);
    }

    [Test]
    public void GivenDateEndModeWithoutEndDate_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            CreateValidCommand(endDate: null, useDefaultEndDate: false)
        );

        result.ShouldHaveValidationErrorFor(c => c.EndDate);
    }

    [Test]
    public void GivenCountEndModeWithoutLessonCount_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            CreateValidCommand(endMode: EndMode.Count, endDate: null, lessonCount: null)
        );

        result.ShouldHaveValidationErrorFor(c => c.LessonCount);
    }

    [Test]
    public void GivenBiweeklyWithoutParity_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            CreateValidCommand(recurrence: RecurrenceType.Biweekly, parity: null)
        );

        result.ShouldHaveValidationErrorFor(c => c.BiweeklyParity);
    }

    [Test]
    public void GivenInvalidSlot_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(CreateValidCommand(slots: [new SlotRequest(-1, -1)]));

        result.ShouldHaveValidationErrorFor("Slots[0].Weekday");
        result.ShouldHaveValidationErrorFor("Slots[0].StartMinutes");
    }

    private static UpdateGroupScheduleSettingsCommand CreateValidCommand(
        Guid? groupId = null,
        RecurrenceType recurrence = RecurrenceType.Weekly,
        short duration = 60,
        EndMode endMode = EndMode.Date,
        DateOnly? endDate = null,
        short? lessonCount = null,
        int? parity = null,
        IReadOnlyList<SlotRequest>? slots = null,
        bool useDefaultEndDate = true
    ) =>
        new(
            groupId ?? Guid.CreateVersion7(),
            recurrence,
            duration,
            endMode,
            useDefaultEndDate ? endDate ?? new DateOnly(2026, 2, 5) : endDate,
            lessonCount,
            parity,
            SkipHolidays: false,
            NotifyStudents: true,
            slots ?? [new SlotRequest(1, 600)]
        );
}
