namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Update;

public sealed class UpdateOrganizationMemberValidatorTests
{
    private readonly UpdateOrganizationMemberValidator _validator = new();

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
    public void GivenEmptyOrganizationMemberRoleId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                OrganizationMemberRoleId = Guid.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.OrganizationMemberRoleId);
    }

    private static UpdateOrganizationMemberCommand BuildValidCommand() =>
        new(Id: Guid.CreateVersion7(), OrganizationMemberRoleId: Guid.CreateVersion7());
}
