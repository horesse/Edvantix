namespace Edvantix.Organizational.UnitTests.Features.Groups.Members.BulkAdd;

public sealed class BulkAddGroupMembersValidatorTests
{
    private readonly BulkAddGroupMembersValidator _validator = new();

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
    public void GivenEmptyItems_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { Items = [] });

        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Test]
    public void GivenItemsCountExceedsHundred_WhenValidating_ThenShouldHaveError()
    {
        var items = Enumerable
            .Range(0, 101)
            .Select(_ => new BulkAddItem(Guid.CreateVersion7(), GroupMemberRole.Student, new DateOnly(2025, 9, 1)))
            .ToList();

        var result = _validator.TestValidate(BuildValidCommand() with { Items = items });

        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Test]
    public void GivenItemsCountExactlyHundred_WhenValidating_ThenShouldNotHaveError()
    {
        var items = Enumerable
            .Range(0, 100)
            .Select(_ => new BulkAddItem(Guid.CreateVersion7(), GroupMemberRole.Student, new DateOnly(2025, 9, 1)))
            .ToList();

        var result = _validator.TestValidate(BuildValidCommand() with { Items = items });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenItemWithEmptyProfileId_WhenValidating_ThenShouldHaveError()
    {
        var items = new[]
        {
            new BulkAddItem(Guid.Empty, GroupMemberRole.Student, new DateOnly(2025, 9, 1)),
        };

        var result = _validator.TestValidate(BuildValidCommand() with { Items = items });

        result.ShouldHaveValidationErrorFor("Items[0].ProfileId");
    }

    [Test]
    public void GivenItemWithDefaultJoinedAt_WhenValidating_ThenShouldHaveError()
    {
        var items = new[]
        {
            new BulkAddItem(Guid.CreateVersion7(), GroupMemberRole.Student, default),
        };

        var result = _validator.TestValidate(BuildValidCommand() with { Items = items });

        result.ShouldHaveValidationErrorFor("Items[0].JoinedAt");
    }

    [Test]
    public void GivenMultipleItemsOneInvalid_WhenValidating_ThenShouldHaveErrorOnlyForThatItem()
    {
        var items = new[]
        {
            new BulkAddItem(Guid.CreateVersion7(), GroupMemberRole.Student, new DateOnly(2025, 9, 1)),
            new BulkAddItem(Guid.Empty, GroupMemberRole.Teacher, new DateOnly(2025, 9, 1)),
        };

        var result = _validator.TestValidate(BuildValidCommand() with { Items = items });

        result.ShouldHaveValidationErrorFor("Items[1].ProfileId");
        result.ShouldNotHaveValidationErrorFor("Items[0].ProfileId");
    }

    private static BulkAddGroupMembersCommand BuildValidCommand() =>
        new(
            Guid.CreateVersion7(),
            [new BulkAddItem(Guid.CreateVersion7(), GroupMemberRole.Student, new DateOnly(2025, 9, 1))]
        );
}
