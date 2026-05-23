namespace Edvantix.Organizational.UnitTests.Features.Roles.Create;

public sealed class CreateRoleValidatorTests
{
    private readonly CreateRoleValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(
            new CreateRoleCommand("Менеджер", "Управление проектами")
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenValidCommandWithNullDescription_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(new CreateRoleCommand("Читатель", null));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyName_WhenValidating_ThenShouldHaveError(string? name)
    {
        var result = _validator.TestValidate(new CreateRoleCommand(name!, null));

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
