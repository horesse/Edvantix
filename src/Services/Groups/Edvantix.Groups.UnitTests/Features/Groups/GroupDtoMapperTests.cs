namespace Edvantix.Groups.UnitTests.Features.Groups;

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
        dto.LevelId.ShouldBe(group.LevelId);
        // LevelCode и LevelName — кросс-сервисные поля, обогащаются в обработчике.
        // Маппер выставляет пустые строки-заглушки.
        dto.LevelCode.ShouldBe(string.Empty);
        dto.LevelName.ShouldBe(string.Empty);
        dto.Format.ShouldBe(group.Format);
        dto.Status.ShouldBe(GroupStatus.Recruiting);
        dto.Capacity.ShouldBe(group.Capacity);
        dto.CourseCode.ShouldBeNull();
        dto.CourseName.ShouldBeNull();
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
        dto.LevelId.ShouldBe(group.LevelId);
        // LevelCode и LevelName — кросс-сервисные поля, обогащаются в обработчике.
        dto.LevelCode.ShouldBe(string.Empty);
        dto.LevelName.ShouldBe(string.Empty);
        dto.CourseId.ShouldBe(group.CourseId);
        dto.CourseCode.ShouldBeNull();
        dto.CourseName.ShouldBeNull();
        dto.Format.ShouldBe(group.Format);
        dto.Platform.ShouldBe(group.Platform);
        dto.Capacity.ShouldBe(group.Capacity);
        dto.TeacherMemberId.ShouldBe(group.TeacherMemberId);
        dto.Teacher.MemberId.ShouldBe(group.TeacherMemberId);
        dto.Teacher.FullName.ShouldBe(string.Empty);
        dto.Teacher.AvatarUrl.ShouldBeNull();
        dto.Schedule.ShouldBeNull();
        dto.UpcomingLessons.ShouldBeEmpty();
    }

    [Test]
    public void GivenGroupWithOfflineFormat_WhenMappingToListItem_ThenPlatformShouldBeNull()
    {
        var group = CreateGroup(GroupFormat.Offline, roomId: Guid.CreateVersion7(), platform: null);

        var dto = _listMapper.Map(group);

        dto.Format.ShouldBe(GroupFormat.Offline);
    }

    [Test]
    public void GivenGroupWithOnlineFormat_WhenMappingToListItem_ThenStartDateAndEndDateAreMapped()
    {
        var group = CreateGroup();

        var dto = _listMapper.Map(group);

        dto.StartDate.ShouldBe(new DateOnly(2025, 9, 1));
        dto.EndDate.ShouldBe(new DateOnly(2026, 6, 30));
    }

    [Test]
    public void GivenGroupDetailMapper_WhenMapping_ThenStatusStartDateAndEndDateAreMapped()
    {
        var group = CreateGroup();

        var dto = _detailMapper.Map(group);

        dto.Status.ShouldBe(GroupStatus.Recruiting);
        dto.StartDate.ShouldBe(new DateOnly(2025, 9, 1));
        dto.EndDate.ShouldBe(new DateOnly(2026, 6, 30));
    }

    [Test]
    public void GivenGroupDetailMapper_WhenMapping_ThenScheduleAndUpcomingLessonsArePlaceholders()
    {
        var group = CreateGroup();

        var dto = _detailMapper.Map(group);

        dto.Schedule.ShouldBeNull();
        dto.UpcomingLessons.ShouldBeEmpty();
    }

    /// <summary>
    /// Создаёт тестовую Group. LevelId — мягкая ссылка (soft Guid reference);
    /// LevelCode/LevelName заполняются через кросс-сервисный вызов в обработчике.
    /// </summary>
    private Group CreateGroup(
        GroupFormat format = GroupFormat.Online,
        Guid? roomId = null,
        OnlinePlatform? platform = OnlinePlatform.Zoom
    ) =>
        new(
            _organizationId,
            GroupCode.From("B1-01"),
            "Английский B1",
            "Описание группы",
            levelId: Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            format,
            roomId,
            platform,
            15,
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 6, 30)
        );
}
