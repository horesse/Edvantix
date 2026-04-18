namespace Edvantix.Organizational.UnitTests.Features.Organizations.Create;

public sealed class CreateOrganizationValidatorTests
{
    private readonly CreateOrganizationValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
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
    public void GivenFutureRegistrationDate_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                RegistrationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.RegistrationDate);
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyPrimaryContactValue_WhenValidating_ThenShouldHaveError(string value)
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                PrimaryContactValue = value,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.PrimaryContactValue);
    }

    [Test]
    public void GivenPrimaryContactValueExceedingMaxLength_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                PrimaryContactValue = new string('а', DataSchemaLength.ExtraLarge + 1),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.PrimaryContactValue);
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyPrimaryContactDescription_WhenValidating_ThenShouldHaveError(string desc)
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                PrimaryContactDescription = desc,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.PrimaryContactDescription);
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
    public void GivenInvalidContactType_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                PrimaryContactType = (ContactType)999,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.PrimaryContactType);
    }

    private static CreateOrganizationCommand BuildValidCommand() =>
        new(
            "ООО Тестовая Организация",
            "ТестОрг",
            IsLegalEntity: true,
            new DateOnly(2020, 1, 1),
            LegalForm.Llc,
            OrganizationType.PrivateEducationalCenter,
            "info@test.com",
            ContactType.Email,
            "Основной контакт"
        );
}
