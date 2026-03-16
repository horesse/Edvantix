using Edvantix.Persona.Features.Profiles.UpdateProfile;

namespace Edvantix.Persona.UnitTests.Features.Profiles.UpdateProfile;

public sealed class UpdateProfileValidatorTests
{
    private readonly UpdateProfileValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyValidationErrors()
    {
        var result = _validator.TestValidate(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyOrNullFirstName_WhenValidating_ThenShouldHaveValidationError(string? firstName)
    {
        var command = BuildValidCommand() with { FirstName = firstName! };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Test]
    public void GivenFirstNameExceedingMaxLength_WhenValidating_ThenShouldHaveValidationError()
    {
        var command = BuildValidCommand() with { FirstName = new string('А', DataSchemaLength.Large + 1) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyOrNullLastName_WhenValidating_ThenShouldHaveValidationError(string? lastName)
    {
        var command = BuildValidCommand() with { LastName = lastName! };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Test]
    public void GivenMiddleNameExceedingMaxLength_WhenValidating_ThenShouldHaveValidationError()
    {
        var command = BuildValidCommand() with { MiddleName = new string('А', DataSchemaLength.Large + 1) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MiddleName);
    }

    [Test]
    public void GivenBioExceedingMaxLength_WhenValidating_ThenShouldHaveValidationError()
    {
        var command = BuildValidCommand() with { Bio = new string('А', 601) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Bio);
    }

    [Test]
    public void GivenBioAtMaxLength_WhenValidating_ThenShouldNotHaveBioValidationError()
    {
        var command = BuildValidCommand() with { Bio = new string('А', 600) };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Bio);
    }

    [Test]
    public void GivenMoreThanMaxSkillsCount_WhenValidating_ThenShouldHaveValidationError()
    {
        var tooManySkills = Enumerable
            .Range(1, Profile.MaxSkillsCount + 1)
            .Select(i => $"Навык {i}")
            .ToList();

        var command = BuildValidCommand() with { Skills = tooManySkills };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Skills);
    }

    [Test]
    public void GivenSkillNameExceedingMaxLength_WhenValidating_ThenShouldHaveValidationError()
    {
        var command = BuildValidCommand() with
        {
            Skills = [new string('А', DataSchemaLength.Large + 1)],
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Skills[0]");
    }

    [Test]
    public void GivenContactWithEmptyValue_WhenValidating_ThenShouldHaveContactValidationError()
    {
        var command = BuildValidCommand() with
        {
            Contacts = [new ContactRequest(ContactType.Email, "")],
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Contacts[0].Value");
    }

    [Test]
    public void GivenContactWithInvalidType_WhenValidating_ThenShouldHaveContactValidationError()
    {
        var command = BuildValidCommand() with
        {
            Contacts = [new ContactRequest((ContactType)99, "test@example.com")],
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Contacts[0].Type");
    }

    [Test]
    public void GivenEducationWithEndDateBeforeStartDate_WhenValidating_ThenShouldHaveValidationError()
    {
        var command = BuildValidCommand() with
        {
            Educations =
            [
                new EducationRequest(
                    DateStart: new DateOnly(2020, 1, 1),
                    Institution: "МГУ",
                    Level: EducationLevel.HigherBachelor,
                    DateEnd: new DateOnly(2019, 1, 1) // до начала
                ),
            ],
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Educations[0]");
    }

    [Test]
    public void GivenEmploymentWithEmptyWorkplace_WhenValidating_ThenShouldHaveValidationError()
    {
        var command = BuildValidCommand() with
        {
            EmploymentHistories =
            [
                new EmploymentHistoryRequest(
                    Workplace: "",
                    Position: "Разработчик",
                    StartDate: DateTime.UtcNow.AddYears(-2)
                ),
            ],
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("EmploymentHistories[0].Workplace");
    }

    [Test]
    public void GivenEmploymentWithEndDateBeforeStartDate_WhenValidating_ThenShouldHaveValidationError()
    {
        var start = DateTime.UtcNow.AddYears(-1);
        var command = BuildValidCommand() with
        {
            EmploymentHistories =
            [
                new EmploymentHistoryRequest(
                    Workplace: "ООО Рога и Копыта",
                    Position: "Разработчик",
                    StartDate: start,
                    EndDate: start.AddMonths(-6) // до начала
                ),
            ],
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("EmploymentHistories[0]");
    }

    private static UpdateProfileCommand BuildValidCommand() =>
        new(
            FirstName: "Иван",
            LastName: "Иванов",
            MiddleName: null,
            BirthDate: new DateOnly(1990, 1, 1),
            Bio: null,
            Contacts: [],
            EmploymentHistories: [],
            Educations: [],
            Skills: []
        );
}
