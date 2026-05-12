namespace Edvantix.Schedule.UnitTests.Features.GroupSchedules.AddException;

public sealed class AddScheduleExceptionValidatorTests
{
    private readonly AddScheduleExceptionValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveErrors()
    {
        var command = new AddScheduleExceptionCommand(
            Guid.CreateVersion7(),
            new DateOnly(2026, 1, 5),
            "reason"
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyGroupId_WhenValidating_ThenShouldHaveError()
    {
        var command = new AddScheduleExceptionCommand(Guid.Empty, new DateOnly(2026, 1, 5), null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.GroupId);
    }

    [Test]
    public void GivenLongReason_WhenValidating_ThenShouldHaveError()
    {
        var command = new AddScheduleExceptionCommand(
            Guid.CreateVersion7(),
            new DateOnly(2026, 1, 5),
            new string('a', 501)
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Reason);
    }
}
