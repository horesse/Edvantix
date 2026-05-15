namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class GroupAddMemberTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();
    private static readonly Guid ValidCourseId = Guid.CreateVersion7();
    private static readonly Guid ValidTeacherId = Guid.CreateVersion7();
    private static readonly Guid ValidRoomId = Guid.CreateVersion7();
    private static readonly GroupCode ValidCode = GroupCode.From("EN-B1-01");
    private static readonly DateOnly StartDate = new(2025, 9, 1);
    private static readonly DateOnly EndDate = new(2026, 6, 30);

    private static Group CreateActiveGroup(int capacity = 3) =>
        new(
            ValidOrgId,
            ValidCode,
            "Тестовая группа",
            "Описание",
            GroupLevel.B1,
            ValidCourseId,
            ValidTeacherId,
            GroupFormat.Online,
            null,
            OnlinePlatform.Zoom,
            capacity,
            StartDate,
            EndDate
        );

    private static GroupMember CreateStudentMember(Guid? profileId = null, Guid? groupId = null) =>
        new(
            ValidOrgId,
            groupId ?? Guid.CreateVersion7(),
            profileId ?? Guid.CreateVersion7(),
            GroupMemberRole.Student,
            StartDate
        );

    [Test]
    public void GivenCapacityReached_WhenAddingStudent_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateActiveGroup(capacity: 2);
        group.AddMember(CreateStudentMember());
        group.AddMember(CreateStudentMember());

        var act = () => group.AddMember(CreateStudentMember());

        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldContain("вместимости");
    }

    [Test]
    public void GivenAlreadyActiveMember_WhenAddingAgain_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateActiveGroup();
        var profileId = Guid.CreateVersion7();
        group.AddMember(CreateStudentMember(profileId));

        var act = () => group.AddMember(CreateStudentMember(profileId));

        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldContain("активным участником");
    }

    [Test]
    public void GivenArchivedGroup_WhenAddingMember_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateActiveGroup();
        group.Archive();

        var act = () => group.AddMember(CreateStudentMember());

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenExitedMember_WhenAddingSameProfileAgain_ThenShouldSucceed()
    {
        var group = CreateActiveGroup(capacity: 5);
        var profileId = Guid.CreateVersion7();
        var sharedGroupId = Guid.CreateVersion7();
        var first = new GroupMember(
            ValidOrgId,
            sharedGroupId,
            profileId,
            GroupMemberRole.Student,
            StartDate
        );
        group.AddMember(first);
        first.Exit(StartDate.AddMonths(1));

        var act = () =>
            group.AddMember(
                new GroupMember(
                    ValidOrgId,
                    sharedGroupId,
                    profileId,
                    GroupMemberRole.Student,
                    StartDate.AddMonths(2)
                )
            );

        act.ShouldNotThrow();
        group.Members.Count(m => m.ProfileId == profileId).ShouldBe(2);
    }

    [Test]
    public void GivenCapacityReached_WhenAddingTeacher_ThenShouldSucceed()
    {
        var group = CreateActiveGroup(capacity: 1);
        group.AddMember(CreateStudentMember(groupId: Guid.CreateVersion7()));

        var teacher = new GroupMember(
            ValidOrgId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupMemberRole.Teacher,
            StartDate
        );

        var act = () => group.AddMember(teacher);

        act.ShouldNotThrow();
    }

    [Test]
    public void GivenActiveGroup_WhenRemovingExistingMember_ThenShouldSetExitedAt()
    {
        var group = CreateActiveGroup();
        var member = CreateStudentMember();
        group.AddMember(member);

        var exitDate = StartDate.AddMonths(1);
        group.RemoveMember(member.Id, exitDate, "Причина");

        member.ExitedAt.ShouldBe(exitDate);
        member.ExitReason.ShouldBe("Причина");
    }

    [Test]
    public void GivenArchivedGroup_WhenRemovingMember_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateActiveGroup();
        var member = CreateStudentMember();
        group.AddMember(member);
        group.Archive();

        var act = () => group.RemoveMember(member.Id, StartDate.AddMonths(1));

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenNonExistentMemberId_WhenRemovingMember_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateActiveGroup();

        var act = () => group.RemoveMember(Guid.CreateVersion7(), StartDate.AddMonths(1));

        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldContain("не найден");
    }

    [Test]
    public void GivenAlreadyExitedMember_WhenRemovingAgain_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateActiveGroup();
        var member = CreateStudentMember();
        group.AddMember(member);
        member.Exit(StartDate.AddMonths(1));

        var act = () => group.RemoveMember(member.Id, StartDate.AddMonths(2));

        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldContain("не найден");
    }
}
