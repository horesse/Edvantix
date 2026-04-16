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

    private static UpdateOrganizationCommand BuildValidCommand() =>
        new(
            Guid.CreateVersion7(),
            "ООО Тестовая Организация",
            "ТестОрг",
            OrganizationType.PrivateEducationalCenter,
            LegalForm.Llc
        );
}
