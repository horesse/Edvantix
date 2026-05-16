using System.Reflection;

namespace Edvantix.Organizational.UnitTests.Features.Groups;

public sealed class GroupDtoMapperTests
{
    private readonly GroupListItemDtoMapper _listMapper = new();
    private readonly GroupDetailDtoMapper _detailMapper = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();

    [Test]
    public void GivenGroup_WhenMappingToListItem_ThenShouldMapAllFields()
    {
        var group = CreateGroupWithLevel();

        var dto = _listMapper.Map(group);

        dto.Id.ShouldBe(group.Id);
        dto.Code.ShouldBe(group.Code.Value);
        dto.Name.ShouldBe(group.Name);
        dto.LevelId.ShouldBe(group.LevelId);
        dto.LevelCode.ShouldBe(group.Level.Code.Value);
        dto.LevelName.ShouldBe(group.Level.Name);
        dto.LevelTone.ShouldBe(group.Level.Tone);
        dto.CourseId.ShouldBe(group.CourseId);
        dto.CourseCode.ShouldBe(string.Empty);
        dto.CourseName.ShouldBe(string.Empty);
        dto.Format.ShouldBe(group.Format);
        dto.Status.ShouldBe(GroupStatus.Recruiting);
        dto.Capacity.ShouldBe(group.Capacity);
        dto.Teacher.MemberId.ShouldBe(group.TeacherMemberId);
        dto.Teacher.FullName.ShouldBe(string.Empty);
        dto.Teacher.PrimaryRole.ShouldBe(string.Empty);
        dto.Teacher.AvatarUrl.ShouldBeNull();
        dto.RoomId.ShouldBeNull();
        dto.RoomLabel.ShouldBeNull();
        dto.ScheduleSummary.ShouldBeNull();
    }

    [Test]
    public void GivenGroup_WhenMappingToDetail_ThenShouldMapAllFields()
    {
        var group = CreateGroupWithLevel();

        var dto = _detailMapper.Map(group);

        dto.Id.ShouldBe(group.Id);
        dto.Code.ShouldBe(group.Code.Value);
        dto.Name.ShouldBe(group.Name);
        dto.Description.ShouldBe(group.Description);
        dto.LevelId.ShouldBe(group.LevelId);
        dto.LevelCode.ShouldBe(group.Level.Code.Value);
        dto.LevelName.ShouldBe(group.Level.Name);
        dto.LevelTone.ShouldBe(group.Level.Tone);
        dto.CourseId.ShouldBe(group.CourseId);
        dto.CourseCode.ShouldBe(string.Empty);
        dto.CourseName.ShouldBe(string.Empty);
        dto.Format.ShouldBe(group.Format);
        dto.Platform.ShouldBe(group.Platform);
        dto.Capacity.ShouldBe(group.Capacity);
        dto.Teacher.MemberId.ShouldBe(group.TeacherMemberId);
        dto.Teacher.FullName.ShouldBe(string.Empty);
        dto.Schedule.ShouldBeNull();
        dto.UpcomingLessons.ShouldBeEmpty();
    }

    [Test]
    public void GivenGroupWithMembers_WhenMappingMemberCount_ThenShouldCountOnlyActiveMembers()
    {
        var group = CreateGroupWithLevel();
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

    [Test]
    public void GivenGroupWithOfflineFormat_WhenMappingToListItem_ThenPlatformShouldBeNull()
    {
        var group = CreateGroupWithLevel(GroupFormat.Offline, roomId: Guid.CreateVersion7(), platform: null);

        var dto = _listMapper.Map(group);

        dto.Format.ShouldBe(GroupFormat.Offline);
        dto.Platform.ShouldBeNull();
    }

    [Test]
    public void GivenGroupWithOnlineFormat_WhenMappingToListItem_ThenStartDateEndDateAndPlatformAreMapped()
    {
        var group = CreateGroupWithLevel();

        var dto = _listMapper.Map(group);

        dto.Platform.ShouldBe(OnlinePlatform.Zoom);
        dto.StartDate.ShouldBe(new DateOnly(2025, 9, 1));
        dto.EndDate.ShouldBe(new DateOnly(2026, 6, 30));
    }

    [Test]
    public void GivenGroupDetailMapper_WhenMapping_ThenStatusStartDateAndEndDateAreMapped()
    {
        var group = CreateGroupWithLevel();

        var dto = _detailMapper.Map(group);

        dto.Status.ShouldBe(GroupStatus.Recruiting);
        dto.StartDate.ShouldBe(new DateOnly(2025, 9, 1));
        dto.EndDate.ShouldBe(new DateOnly(2026, 6, 30));
    }

    [Test]
    public void GivenGroupWithNoMembers_WhenMappingToDetail_ThenMemberCountIsZero()
    {
        var group = CreateGroupWithLevel();

        var dto = _detailMapper.Map(group);

        dto.MemberCount.ShouldBe(0);
    }

    [Test]
    public void GivenGroupDetailMapper_WhenMapping_ThenScheduleAndUpcomingLessonsArePlaceholders()
    {
        var group = CreateGroupWithLevel();

        var dto = _detailMapper.Map(group);

        dto.Schedule.ShouldBeNull();
        dto.UpcomingLessons.ShouldBeEmpty();
    }

    [Test]
    public void GivenGroupWithMembers_WhenMappingToDetail_ThenShouldCountOnlyActiveMembers()
    {
        var group = CreateGroupWithLevel();
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

        var dto = _detailMapper.Map(group);

        dto.MemberCount.ShouldBe(1);
    }

    /// <summary>
    /// Создаёт Group с заполненным navigation property Level через reflection.
    /// В production Level всегда подгружается EF через AutoInclude.
    /// Entity.Id по умолчанию Guid.Empty, поэтому устанавливаем Id Level-а через reflection
    /// перед тем, как передать его в конструктор Group.
    /// </summary>
    private Group CreateGroupWithLevel(
        GroupFormat format = GroupFormat.Online,
        Guid? roomId = null,
        OnlinePlatform? platform = OnlinePlatform.Zoom
    )
    {
        var levelId = Guid.CreateVersion7();

        var level = new Level(
            _organizationId,
            LevelCode.From("B1"),
            "B1 — Средний",
            null,
            LevelTone.Blue,
            2
        );

        // Entity.Id — Guid.Empty без EF; устанавливаем вручную.
        typeof(Level)
            .GetProperty(nameof(Level.Id), BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(level, levelId);

        var group = new Group(
            _organizationId,
            GroupCode.From("B1-01"),
            "Английский B1",
            "Описание группы",
            levelId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            format,
            roomId,
            platform,
            15,
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 6, 30)
        );

        // Level подгружается EF через AutoInclude; в unit-тестах устанавливаем через reflection.
        typeof(Group)
            .GetProperty(nameof(Group.Level), BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(group, level);

        return group;
    }
}
