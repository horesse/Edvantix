namespace Edvantix.Groups.UnitTests.Features.Levels.Update;

public sealed class UpdateLevelValidatorTests
{
    private readonly UpdateLevelValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Id = Guid.Empty });

        result.ShouldHaveValidationErrorFor(x => x.Id);
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
    public void GivenTooLongName_WhenValidating_ThenShouldHaveError()
    {
        var name = new string('A', 65); // max is 64

        var result = _validator.TestValidate(BuildValidCommand() with { Name = name });

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void GivenMaxLengthName_WhenValidating_ThenShouldNotHaveError()
    {
        var name = new string('A', 64);

        var result = _validator.TestValidate(BuildValidCommand() with { Name = name });

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void GivenTooLongDescription_WhenValidating_ThenShouldHaveError()
    {
        var description = new string('X', 257); // max is 256

        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Description = description,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void GivenMaxLengthDescription_WhenValidating_ThenShouldNotHaveError()
    {
        var description = new string('X', 256);

        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                Description = description,
            }
        );

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void GivenNullDescription_WhenValidating_ThenShouldNotHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Description = null });

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    private static UpdateLevelCommand BuildValidCommand() =>
        new(
            Id: Guid.CreateVersion7(),
            Name: "Intermediate",
            Description: "Средний уровень",
            Tone: LevelTone.Teal,
            SortOrder: 2
        );
}
