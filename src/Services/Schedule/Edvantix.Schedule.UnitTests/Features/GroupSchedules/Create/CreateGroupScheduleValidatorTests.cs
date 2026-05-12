namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.Create;

public sealed class CreateGroupScheduleValidatorTests
{
    private readonly CreateGroupScheduleValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveErrors()
    {
        var result = _validator.TestValidate(CreateValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyGroupId_WhenValidating_ThenShouldHaveError()
    {
        var command = CreateValidCommand(groupId: Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.GroupId);
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenValidating_ThenShouldHaveError()
    {
        var command = CreateValidCommand(organizationId: Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.OrganizationId);
    }

    [Test]
    public void GivenInvalidDuration_WhenValidating_ThenShouldHaveError()
    {
        var command = CreateValidCommand(duration: 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.LessonDurationMinutes);
    }

    [Test]
    public void GivenDateEndModeWithoutEndDate_WhenValidating_ThenShouldHaveError()
    {
        var command = CreateValidCommand(endDate: null, useDefaultEndDate: false);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.EndDate);
    }

    [Test]
    public void GivenDateEndModeWithEndDateBeforeStart_WhenValidating_ThenShouldHaveError()
    {
        var command = CreateValidCommand(endDate: new DateOnly(2026, 1, 1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.EndDate);
    }

    [Test]
    public void GivenCountEndModeWithoutLessonCount_WhenValidating_ThenShouldHaveError()
    {
        var command = CreateValidCommand(endMode: EndMode.Count, endDate: null, lessonCount: null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.LessonCount);
    }

    [Test]
    public void GivenBiweeklyWithoutParity_WhenValidating_ThenShouldHaveError()
    {
        var command = CreateValidCommand(recurrence: RecurrenceType.Biweekly, parity: null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.BiweeklyParity);
    }

    [Test]
    public void GivenInvalidSlot_WhenValidating_ThenShouldHaveError()
    {
        var command = CreateValidCommand(slots: [new SlotRequest(7, 1440)]);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Slots[0].Weekday");
        result.ShouldHaveValidationErrorFor("Slots[0].StartMinutes");
    }

    internal static CreateGroupScheduleCommand CreateValidCommand(
        Guid? groupId = null,
        Guid? organizationId = null,
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
            organizationId ?? Guid.CreateVersion7(),
            recurrence,
            duration,
            new DateOnly(2026, 1, 5),
            endMode,
            useDefaultEndDate ? endDate ?? new DateOnly(2026, 2, 5) : endDate,
            lessonCount,
            parity,
            SkipHolidays: false,
            NotifyStudents: true,
            slots ?? [new SlotRequest(1, 600)],
            HolidayCountryCode: null
        );
}
