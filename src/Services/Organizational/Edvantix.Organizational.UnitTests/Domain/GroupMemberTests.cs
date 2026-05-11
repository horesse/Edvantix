namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class GroupMemberTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();
    private static readonly Guid ValidGroupId = Guid.CreateVersion7();
    private static readonly Guid ValidProfileId = Guid.CreateVersion7();
    private static readonly DateOnly ValidJoinDate = new(2024, 9, 1);

    private static GroupMember CreateValidMember() =>
        new(ValidOrgId, ValidGroupId, ValidProfileId, GroupMemberRole.Student, ValidJoinDate)
        {
            Id = Guid.CreateVersion7(),
        };

    [Test]
    public void GivenValidData_WhenCreatingGroupMember_ThenShouldInitializePropertiesCorrectly()
    {
        var member = new GroupMember(
            ValidOrgId,
            ValidGroupId,
            ValidProfileId,
            GroupMemberRole.Teacher,
            ValidJoinDate
        );

        member.OrganizationId.ShouldBe(ValidOrgId);
        member.GroupId.ShouldBe(ValidGroupId);
        member.ProfileId.ShouldBe(ValidProfileId);
        member.Role.ShouldBe(GroupMemberRole.Teacher);
        member.JoinedAt.ShouldBe(ValidJoinDate);
        member.ExitedAt.ShouldBeNull();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingGroupMember_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new GroupMember(Guid.Empty, ValidGroupId, ValidProfileId, GroupMemberRole.Student, ValidJoinDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyGroupId_WhenCreatingGroupMember_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new GroupMember(ValidOrgId, Guid.Empty, ValidProfileId, GroupMemberRole.Student, ValidJoinDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyProfileId_WhenCreatingGroupMember_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new GroupMember(ValidOrgId, ValidGroupId, Guid.Empty, GroupMemberRole.Student, ValidJoinDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidExitDate_WhenExiting_ThenShouldSetExitedAtAndReason()
    {
        var member = CreateValidMember();
        var exitDate = ValidJoinDate.AddMonths(3);

        member.Exit(exitDate, "Завершение курса");

        member.ExitedAt.ShouldBe(exitDate);
        member.ExitReason.ShouldBe("Завершение курса");
    }

    [Test]
    public void GivenExitWithoutReason_WhenExiting_ThenExitReasonShouldBeNull()
    {
        var member = CreateValidMember();

        member.Exit(ValidJoinDate.AddMonths(1));

        member.ExitedAt.ShouldNotBeNull();
        member.ExitReason.ShouldBeNull();
    }
}
