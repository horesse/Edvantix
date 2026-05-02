namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class GroupMemberTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();
    private static readonly Guid ValidGroupId = Guid.CreateVersion7();
    private static readonly Guid ValidProfileId = Guid.CreateVersion7();
    private static readonly DateOnly ValidJoinDate = new(2024, 9, 1);

    private static GroupMember CreateValidMember()
    {
        var member = new GroupMember(ValidOrgId, ValidGroupId, ValidProfileId, ValidJoinDate)
        {
            Id = Guid.CreateVersion7(),
        };

        return member;
    }

    [Test]
    public void GivenValidData_WhenCreatingGroupMember_ThenShouldInitializePropertiesCorrectly()
    {
        var member = new GroupMember(ValidOrgId, ValidGroupId, ValidProfileId, ValidJoinDate);

        member.OrganizationId.ShouldBe(ValidOrgId);
        member.GroupId.ShouldBe(ValidGroupId);
        member.ProfileId.ShouldBe(ValidProfileId);
        member.Status.ShouldBe(OrganizationStatus.Active);
        member.JoinedAt.ShouldBe(ValidJoinDate);
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingGroupMember_ThenShouldThrowArgumentException()
    {
        var act = () => new GroupMember(Guid.Empty, ValidGroupId, ValidProfileId, ValidJoinDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyGroupId_WhenCreatingGroupMember_ThenShouldThrowArgumentException()
    {
        var act = () => new GroupMember(ValidOrgId, Guid.Empty, ValidProfileId, ValidJoinDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyProfileId_WhenCreatingGroupMember_ThenShouldThrowArgumentException()
    {
        var act = () => new GroupMember(ValidOrgId, ValidGroupId, Guid.Empty, ValidJoinDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidExitDate_WhenExiting_ThenShouldSetArchivedStatusAndMarkAsDeleted()
    {
        var member = CreateValidMember();
        var exitDate = ValidJoinDate.AddMonths(3);

        member.Exit(exitDate, "Завершение курса");

        member.Status.ShouldBe(OrganizationStatus.Archived);
        member.ExitedAt.ShouldBe(exitDate);
    }
}
