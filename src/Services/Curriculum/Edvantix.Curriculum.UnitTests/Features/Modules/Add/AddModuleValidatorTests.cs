namespace Edvantix.Curriculum.UnitTests.Features.Modules.Add;

public sealed class AddModuleValidatorTests
{
    private readonly AddModuleValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyCourseId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { CourseId = Guid.Empty });

        result.ShouldHaveValidationErrorFor(x => x.CourseId);
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyName_WhenValidating_ThenShouldHaveError(string name)
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Name = name });

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void GivenNameExceedingMaxLength_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Name = new string('A', 513),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void GivenSummaryExceedingMaxLength_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Summary = new string('A', 1025),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Summary);
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    public void GivenNonPositiveWeeks_WhenValidating_ThenShouldHaveError(short weeks)
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Weeks = weeks });

        result.ShouldHaveValidationErrorFor(x => x.Weeks);
    }

    private static AddModuleCommand BuildValidCommand() =>
        new(Guid.CreateVersion7(), "Module 1", "Summary", Weeks: 2);
}
