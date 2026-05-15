namespace Edvantix.Organizational.UnitTests.Features.Groups.Members;

public sealed class GroupMemberDomainToDtoMapperTests
{
    private readonly GroupMemberDomainToDtoMapper _mapper = new();

    [Test]
    public void GivenActiveMember_WhenMapping_ThenShouldMapAllFieldsWithEmptyProfile()
    {
        var orgId = Guid.CreateVersion7();
        var groupId = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        var joinedAt = new DateOnly(2025, 9, 1);
        var member = new GroupMember(orgId, groupId, profileId, GroupMemberRole.Student, joinedAt);

        var dto = _mapper.Map(member);

        dto.Id.ShouldBe(member.Id);
        dto.ProfileId.ShouldBe(profileId);
        dto.FullName.ShouldBe(string.Empty);
        dto.AvatarUrl.ShouldBeNull();
        dto.Role.ShouldBe(GroupMemberRole.Student);
        dto.JoinedAt.ShouldBe(joinedAt);
        dto.ExitedAt.ShouldBeNull();
        dto.ExitReason.ShouldBeNull();
    }

    [Test]
    public void GivenTeacherRole_WhenMapping_ThenShouldMapRoleCorrectly()
    {
        var member = new GroupMember(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupMemberRole.Teacher,
            new DateOnly(2025, 9, 1)
        );

        var dto = _mapper.Map(member);

        dto.Role.ShouldBe(GroupMemberRole.Teacher);
    }

    [Test]
    public void GivenExitedMember_WhenMapping_ThenShouldMapExitedAtAndExitReason()
    {
        var joinedAt = new DateOnly(2025, 9, 1);
        var exitedAt = new DateOnly(2025, 11, 1);
        const string reason = "Отчисление";
        var member = new GroupMember(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupMemberRole.Student,
            joinedAt
        );
        member.Exit(exitedAt, reason);

        var dto = _mapper.Map(member);

        dto.ExitedAt.ShouldBe(exitedAt);
        dto.ExitReason.ShouldBe(reason);
        dto.JoinedAt.ShouldBe(joinedAt);
    }

    [Test]
    public void GivenExitedMemberWithoutReason_WhenMapping_ThenShouldMapExitReasonAsNull()
    {
        var member = new GroupMember(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupMemberRole.Student,
            new DateOnly(2025, 9, 1)
        );
        member.Exit(new DateOnly(2025, 11, 1));

        var dto = _mapper.Map(member);

        dto.ExitedAt.ShouldNotBeNull();
        dto.ExitReason.ShouldBeNull();
    }
}
