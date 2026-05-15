namespace Edvantix.Organizational.UnitTests.Features.Groups.Members.Add;

public sealed class AddGroupMemberValidatorTests
{
    private readonly AddGroupMemberValidator _validator = new();

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyGroupId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { GroupId = Guid.Empty });

        result.ShouldHaveValidationErrorFor(x => x.GroupId);
    }

    [Test]
    public void GivenEmptyProfileId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { ProfileId = Guid.Empty });

        result.ShouldHaveValidationErrorFor(x => x.ProfileId);
    }

    [Test]
    public void GivenDefaultJoinedAt_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { JoinedAt = default });

        result.ShouldHaveValidationErrorFor(x => x.JoinedAt);
    }

    private static AddGroupMemberCommand BuildValidCommand() =>
        new(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupMemberRole.Student,
            new DateOnly(2025, 9, 1)
        );
}
