namespace Edvantix.Curriculum.UnitTests.Features.Lessons.Add;

public sealed class AddLessonValidatorTests
{
    private readonly AddLessonValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyModuleId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { ModuleId = Guid.Empty });

        result.ShouldHaveValidationErrorFor(x => x.ModuleId);
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyTitle_WhenValidating_ThenShouldHaveError(string title)
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Title = title });

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    public void GivenTitleExceedingMaxLength_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Title = new string('A', 513),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    [Arguments((short)0)]
    [Arguments((short)-1)]
    public void GivenNonPositiveMinutes_WhenValidating_ThenShouldHaveError(short minutes)
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Minutes = minutes });

        result.ShouldHaveValidationErrorFor(x => x.Minutes);
    }

    [Test]
    public void GivenInvalidType_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Type = (LessonType)999 });

        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Test]
    public void GivenObjectiveExceedingMaxLength_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Objectives = [new string('A', 513)],
            }
        );

        result.ShouldHaveValidationErrorFor("Objectives[0]");
    }

    private static AddLessonCommand BuildValidCommand() =>
        new(
            Guid.CreateVersion7(),
            "Lesson 1",
            LessonType.Lecture,
            Minutes: 45,
            ["Understand basics"]
        );
}
