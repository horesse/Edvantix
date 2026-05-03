namespace Edvantix.Organizational.UnitTests.Features.Roles.Update;

public sealed class UpdateRoleValidatorTests
{
    private readonly UpdateRoleValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(
            new UpdateRoleCommand(Guid.CreateVersion7(), "Менеджер", "Управление проектами")
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(new UpdateRoleCommand(Guid.Empty, "Менеджер", null));

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyName_WhenValidating_ThenShouldHaveError(string? name)
    {
        var result = _validator.TestValidate(
            new UpdateRoleCommand(Guid.CreateVersion7(), name!, null)
        );

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
