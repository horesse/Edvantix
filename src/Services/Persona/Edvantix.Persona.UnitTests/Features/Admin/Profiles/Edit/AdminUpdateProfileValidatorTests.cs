using Edvantix.Persona.Features.Admin.Profiles.Edit;

namespace Edvantix.Persona.UnitTests.Features.Admin.Profiles.Edit;

public sealed class AdminUpdateProfileValidatorTests
{
    private readonly AdminUpdateProfileValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var command = BuildValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyFirstName_WhenValidating_ThenShouldHaveError()
    {
        var command = BuildValidCommand() with { FirstName = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Test]
    public void GivenEmptyLastName_WhenValidating_ThenShouldHaveError()
    {
        var command = BuildValidCommand() with { LastName = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Test]
    public void GivenInvalidFirstNameFormat_WhenValidating_ThenShouldHaveError()
    {
        var command = BuildValidCommand() with { FirstName = "ivan" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Test]
    public void GivenInvalidLastNameFormat_WhenValidating_ThenShouldHaveError()
    {
        var command = BuildValidCommand() with { LastName = "ivanov123" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Test]
    public void GivenValidMiddleName_WhenValidating_ThenShouldNotHaveError()
    {
        var command = BuildValidCommand() with { MiddleName = "Петрович" };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MiddleName);
    }

    [Test]
    public void GivenInvalidMiddleName_WhenValidating_ThenShouldHaveError()
    {
        var command = BuildValidCommand() with { MiddleName = "petrovitch" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MiddleName);
    }

    [Test]
    public void GivenNullMiddleName_WhenValidating_ThenShouldNotHaveError()
    {
        var command = BuildValidCommand() with { MiddleName = null };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MiddleName);
    }

    [Test]
    public void GivenBioExceeding600Chars_WhenValidating_ThenShouldHaveError()
    {
        var command = BuildValidCommand() with { Bio = new string('а', 601) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Bio);
    }

    [Test]
    public void GivenEmptyReason_WhenValidating_ThenShouldHaveError()
    {
        var command = BuildValidCommand() with { Reason = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Test]
    public void GivenReasonExceeding500Chars_WhenValidating_ThenShouldHaveError()
    {
        var command = BuildValidCommand() with { Reason = new string('а', 501) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Test]
    public void GivenTooManySkills_WhenValidating_ThenShouldHaveError()
    {
        var skills = Enumerable
            .Range(0, Profile.MaxSkillsCount + 1)
            .Select(i => $"Навык{i}")
            .ToList();
        var command = BuildValidCommand() with { Skills = skills };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Skills);
    }

    [Test]
    public void GivenEmptySkillName_WhenValidating_ThenShouldHaveError()
    {
        var command = BuildValidCommand() with { Skills = [""] };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Skills[0]");
    }

    [Test]
    public void GivenHyphenatedName_WhenValidating_ThenShouldNotHaveError()
    {
        var command = BuildValidCommand() with { FirstName = "Анна-Мария" };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
    }

    private static AdminUpdateProfileCommand BuildValidCommand() =>
        new(
            Guid.CreateVersion7(),
            "Анна",
            "Петрова",
            null,
            new DateOnly(1995, 3, 20),
            "Описание",
            [],
            [],
            [],
            [],
            "Причина изменения"
        );
}
