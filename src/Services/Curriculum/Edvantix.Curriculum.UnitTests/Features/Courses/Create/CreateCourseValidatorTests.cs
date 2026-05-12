namespace Edvantix.Curriculum.UnitTests.Features.Courses.Create;

public sealed class CreateCourseValidatorTests
{
    private readonly CreateCourseValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyCode_WhenValidating_ThenShouldHaveError(string code)
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Code = code });

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Test]
    [Arguments("RU_101")]
    [Arguments("RU 101")]
    [Arguments("РУ-101")]
    public void GivenInvalidCodeFormat_WhenValidating_ThenShouldHaveError(string code)
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Code = code });

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Test]
    public void GivenCodeExceedingMaxLength_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Code = new string('A', 33),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Code);
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
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyLevel_WhenValidating_ThenShouldHaveError(string level)
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Level = level });

        result.ShouldHaveValidationErrorFor(x => x.Level);
    }

    [Test]
    public void GivenLevelExceedingMaxLength_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Level = new string('A', 17),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Level);
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    public void GivenNonPositiveDuration_WhenValidating_ThenShouldHaveError(short durationWeeks)
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                DurationWeeks = durationWeeks,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.DurationWeeks);
    }

    [Test]
    public void GivenEmptyOwnerMemberId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                OwnerMemberId = Guid.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.OwnerMemberId);
    }

    [Test]
    public void GivenDescriptionExceedingMaxLength_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Description = new string('A', 4097),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void GivenInvalidSubject_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Subject = (CourseSubject)999,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Subject);
    }

    private static CreateCourseCommand BuildValidCommand() =>
        new(
            "EN-GEN-B1",
            "English General B1",
            CourseSubject.English,
            "B1",
            DurationWeeks: 12,
            Guid.CreateVersion7(),
            "General English course"
        );
}
