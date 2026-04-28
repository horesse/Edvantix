namespace Edvantix.Organizational.UnitTests.Features.Organizations.Update;

public sealed class UpdateOrganizationValidatorTests
{
    private readonly UpdateOrganizationValidator _validator = new();

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
    public void GivenEmptyFullLegalName_WhenValidating_ThenShouldHaveError(string name)
    {
        var result = _validator.TestValidate(BuildValidCommand() with { FullLegalName = name });

        result.ShouldHaveValidationErrorFor(x => x.FullLegalName);
    }

    [Test]
    public void GivenFullLegalNameExceedingMaxLength_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                FullLegalName = new string('А', DataSchemaLength.SuperLarge + 1),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.FullLegalName);
    }

    [Test]
    public void GivenShortNameExceedingMaxLength_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                ShortName = new string('А', DataSchemaLength.Large + 1),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.ShortName);
    }

    [Test]
    public void GivenNullShortName_WhenValidating_ThenShouldNotHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { ShortName = null });

        result.ShouldNotHaveValidationErrorFor(x => x.ShortName);
    }

    [Test]
    public void GivenInvalidOrganizationType_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                OrganizationType = (OrganizationType)999,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.OrganizationType);
    }

    [Test]
    public void GivenInvalidLegalForm_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                LegalForm = (LegalForm)999,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.LegalForm);
    }

    [Test]
    public void GivenFutureRegistrationDate_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                RegistrationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.RegistrationDate);
    }

    [Test]
    public void GivenTodayRegistrationDate_WhenValidating_ThenShouldNotHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
            }
        );

        result.ShouldNotHaveValidationErrorFor(x => x.RegistrationDate);
    }

    [Test]
    public void GivenEmptyContactValue_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { ContactValue = "" });

        result.ShouldHaveValidationErrorFor(x => x.ContactValue);
    }

    [Test]
    public void GivenEmptyContactDescription_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { ContactDescription = "" });

        result.ShouldHaveValidationErrorFor(x => x.ContactDescription);
    }

    [Test]
    public void GivenInvalidContactType_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                ContactType = (ContactType)999,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.ContactType);
    }

    private static UpdateOrganizationCommand BuildValidCommand() =>
        new(
            Guid.CreateVersion7(),
            "ООО Тестовая Организация",
            "ТестОрг",
            OrganizationType.PrivateEducationalCenter,
            LegalForm.Llc,
            new DateOnly(2020, 1, 15),
            ContactType.Email,
            "test@example.com",
            "Основной контакт директора"
        );
}
