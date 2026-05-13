namespace Edvantix.Organizational.UnitTests.Features.Groups;

public sealed class GroupDtoMapperTests
{
    private readonly GroupListItemDtoMapper _listMapper = new();
    private readonly GroupDetailDtoMapper _detailMapper = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();

    [Test]
    public void GivenGroup_WhenMappingToListItem_ThenShouldMapAllFields()
    {
        var group = CreateGroup();

        var dto = _listMapper.Map(group);

        dto.Id.ShouldBe(group.Id);
        dto.Code.ShouldBe(group.Code.Value);
        dto.Name.ShouldBe(group.Name);
        dto.Level.ShouldBe(group.Level);
        dto.Format.ShouldBe(group.Format);
        dto.Status.ShouldBe(GroupStatus.Recruiting);
        dto.Capacity.ShouldBe(group.Capacity);
        dto.CourseId.ShouldBe(group.CourseId);
        dto.Teacher.MemberId.ShouldBe(group.TeacherMemberId);
        dto.Teacher.FullName.ShouldBe(string.Empty);
        dto.Teacher.PrimaryRole.ShouldBe(string.Empty);
        dto.Teacher.AvatarUrl.ShouldBeNull();
        dto.RoomId.ShouldBeNull();
        dto.RoomLabel.ShouldBeNull();
    }

    [Test]
    public void GivenGroup_WhenMappingToDetail_ThenShouldMapAllFields()
    {
        var group = CreateGroup();

        var dto = _detailMapper.Map(group);

        dto.Id.ShouldBe(group.Id);
        dto.Code.ShouldBe(group.Code.Value);
        dto.Name.ShouldBe(group.Name);
        dto.Description.ShouldBe(group.Description);
        dto.Level.ShouldBe(group.Level);
        dto.Format.ShouldBe(group.Format);
        dto.Platform.ShouldBe(group.Platform);
        dto.Capacity.ShouldBe(group.Capacity);
        dto.CourseId.ShouldBe(group.CourseId);
        dto.Teacher.MemberId.ShouldBe(group.TeacherMemberId);
        dto.Teacher.FullName.ShouldBe(string.Empty);
    }

    [Test]
    public void GivenGroupWithMembers_WhenMappingMemberCount_ThenShouldCountOnlyActiveMembers()
    {
        var group = CreateGroup();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var activeMember = new GroupMember(
            _organizationId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupMemberRole.Student,
            today
        );
        var exitedMember = new GroupMember(
            _organizationId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupMemberRole.Student,
            today.AddDays(-30)
        );

        exitedMember.Exit(today);
        group.AddMember(activeMember);
        group.AddMember(exitedMember);

        var dto = _listMapper.Map(group);

        dto.MemberCount.ShouldBe(1);
    }

    private Group CreateGroup() =>
        new(
            _organizationId,
            GroupCode.From("B1-01"),
            "Английский B1",
            "Описание группы",
            GroupLevel.B1,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupFormat.Online,
            null,
            OnlinePlatform.Zoom,
            15,
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 6, 30)
        );
}
