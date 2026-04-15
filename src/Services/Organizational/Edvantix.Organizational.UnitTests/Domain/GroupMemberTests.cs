namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class GroupMemberTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();
    private static readonly Guid ValidGroupId = Guid.CreateVersion7();
    private static readonly Guid ValidProfileId = Guid.CreateVersion7();
    private static readonly Guid ValidGroupRoleId = Guid.CreateVersion7();
    private static readonly DateOnly ValidJoinDate = new(2024, 9, 1);

    private static GroupMember CreateValidMember()
    {
        var member = new GroupMember(
            ValidOrgId,
            ValidGroupId,
            ValidProfileId,
            ValidGroupRoleId,
            ValidJoinDate
        )
        {
            Id = Guid.CreateVersion7(),
        };

        return member;
    }

    [Test]
    public void GivenValidData_WhenCreatingGroupMember_ThenShouldInitializePropertiesCorrectly()
    {
        var member = new GroupMember(
            ValidOrgId,
            ValidGroupId,
            ValidProfileId,
            ValidGroupRoleId,
            ValidJoinDate
        );

        member.OrganizationId.ShouldBe(ValidOrgId);
        member.GroupId.ShouldBe(ValidGroupId);
        member.ProfileId.ShouldBe(ValidProfileId);
        member.GroupRoleId.ShouldBe(ValidGroupRoleId);
        member.Status.ShouldBe(OrganizationStatus.Active);
        member.IsDeleted.ShouldBeFalse();
    }

    // TODO: Переезд на события
    // [Test]
    // public void GivenValidData_WhenCreatingGroupMember_ThenShouldCreateInitialHistoryEntry()
    // {
    //     var member = CreateValidMember();
    //
    //     member.History.ShouldHaveSingleItem();
    //     member.History[0].JoinedAt.ShouldBe(ValidJoinDate);
    //     member.History[0].ExitedAt.ShouldBeNull();
    // }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingGroupMember_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new GroupMember(
                Guid.Empty,
                ValidGroupId,
                ValidProfileId,
                ValidGroupRoleId,
                ValidJoinDate
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyGroupId_WhenCreatingGroupMember_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new GroupMember(
                ValidOrgId,
                Guid.Empty,
                ValidProfileId,
                ValidGroupRoleId,
                ValidJoinDate
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyProfileId_WhenCreatingGroupMember_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new GroupMember(ValidOrgId, ValidGroupId, Guid.Empty, ValidGroupRoleId, ValidJoinDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyGroupRoleId_WhenCreatingGroupMember_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new GroupMember(ValidOrgId, ValidGroupId, ValidProfileId, Guid.Empty, ValidJoinDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNewRoleId_WhenChangingRole_ThenShouldUpdateGroupRoleId()
    {
        var member = CreateValidMember();
        var newRoleId = Guid.CreateVersion7();

        member.ChangeRole(newRoleId);

        member.GroupRoleId.ShouldBe(newRoleId);
    }

    [Test]
    public void GivenEmptyRoleId_WhenChangingRole_ThenShouldThrowArgumentException()
    {
        var member = CreateValidMember();

        var act = () => member.ChangeRole(Guid.Empty);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidExitDate_WhenExiting_ThenShouldSetArchivedStatusAndMarkAsDeleted()
    {
        var member = CreateValidMember();
        var exitDate = ValidJoinDate.AddMonths(3);

        member.Exit(exitDate, "Завершение курса");

        member.Status.ShouldBe(OrganizationStatus.Archived);
        member.IsDeleted.ShouldBeTrue();
    }

    // TODO: Доменные события
    // [Test]
    // public void GivenValidExitDate_WhenExiting_ThenShouldUpdateHistoryEntry()
    // {
    //     var member = CreateValidMember();
    //     var exitDate = ValidJoinDate.AddMonths(3);
    //
    //     member.Exit(exitDate, "Завершение курса");
    //
    //     var historyEntry = member.History[0];
    //     historyEntry.ExitedAt.ShouldBe(exitDate);
    //     historyEntry.ExitReason.ShouldBe("Завершение курса");
    // }

    [Test]
    public void GivenActiveMember_WhenDeleting_ThenIsDeletedShouldBeTrue()
    {
        var member = CreateValidMember();

        member.Delete();

        member.IsDeleted.ShouldBeTrue();
    }
}
