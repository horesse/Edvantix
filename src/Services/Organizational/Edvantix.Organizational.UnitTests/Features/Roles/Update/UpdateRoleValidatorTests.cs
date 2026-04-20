namespace Edvantix.Organizational.UnitTests.Features.Roles.Update;

public sealed class UpdateRoleValidatorTests
{
    private readonly UpdateRoleValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(
            new UpdateRoleCommand(Guid.CreateVersion7(), "manager", "Менеджер")
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(new UpdateRoleCommand(Guid.Empty, "manager", null));

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenEmptyCode_WhenValidating_ThenShouldHaveError(string? code)
    {
        var result = _validator.TestValidate(
            new UpdateRoleCommand(Guid.CreateVersion7(), code!, null)
        );

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }
}
