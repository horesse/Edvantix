namespace Edvantix.Organizational.UnitTests.Features.Roles.Create;

public sealed class CreateRoleValidatorTests
{
    private readonly CreateRoleValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(new CreateRoleCommand("manager", "Менеджер"));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenValidCommandWithNullDescription_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(new CreateRoleCommand("viewer", null));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyCode_WhenValidating_ThenShouldHaveError(string? code)
    {
        var result = _validator.TestValidate(new CreateRoleCommand(code!, null));

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }
}
