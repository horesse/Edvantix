namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class GroupAggregateTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();
    private static readonly Guid ValidCourseId = Guid.CreateVersion7();
    private static readonly Guid ValidTeacherId = Guid.CreateVersion7();
    private static readonly Guid ValidRoomId = Guid.CreateVersion7();
    private static readonly Guid ValidLevelId = Guid.CreateVersion7();
    private static readonly GroupCode ValidCode = GroupCode.From("EN-B1-12");
    private static readonly DateOnly ValidStartDate = new(2024, 9, 1);
    private static readonly DateOnly ValidEndDate = new(2025, 6, 30);

    private static Group CreateValidOfflineGroup() =>
        new(
            ValidOrgId,
            ValidCode,
            "Английский B1 — группа 12",
            "Курс английского языка уровня B1",
            ValidLevelId,
            ValidCourseId,
            ValidTeacherId,
            GroupFormat.Offline,
            ValidRoomId,
            null,
            15,
            ValidStartDate,
            ValidEndDate
        );

    private static Group CreateValidOnlineGroup() =>
        new(
            ValidOrgId,
            ValidCode,
            "Английский B1 — онлайн",
            "Онлайн-группа уровня B1",
            ValidLevelId,
            ValidCourseId,
            ValidTeacherId,
            GroupFormat.Online,
            null,
            OnlinePlatform.Zoom,
            15,
            ValidStartDate,
            ValidEndDate
        );

    [Test]
    public void GivenValidOfflineData_WhenCreatingGroup_ThenShouldInitializePropertiesCorrectly()
    {
        var group = CreateValidOfflineGroup();

        group.OrganizationId.ShouldBe(ValidOrgId);
        group.Code.ShouldBe(ValidCode);
        group.Name.ShouldBe("Английский B1 — группа 12");
        group.LevelId.ShouldBe(ValidLevelId);
        group.CourseId.ShouldBe(ValidCourseId);
        group.TeacherMemberId.ShouldBe(ValidTeacherId);
        group.Format.ShouldBe(GroupFormat.Offline);
        group.RoomId.ShouldBe(ValidRoomId);
        group.Platform.ShouldBeNull();
        group.Capacity.ShouldBe(15);
        group.StartDate.ShouldBe(ValidStartDate);
        group.EndDate.ShouldBe(ValidEndDate);
        group.Status.ShouldBe(GroupStatus.Recruiting);
        group.IsDeleted.ShouldBeFalse();
        group.Members.ShouldBeEmpty();
    }

    [Test]
    public void GivenOnlineFormat_WhenCreatingGroup_ThenRoomIdShouldBeNullAndPlatformSet()
    {
        var group = CreateValidOnlineGroup();

        group.Format.ShouldBe(GroupFormat.Online);
        group.RoomId.ShouldBeNull();
        group.Platform.ShouldBe(OnlinePlatform.Zoom);
    }

    [Test]
    public void GivenMixedFormat_WhenCreatingGroup_ThenBothRoomAndPlatformRequired()
    {
        var group = new Group(
            ValidOrgId,
            ValidCode,
            "Смешанный формат",
            "Описание",
            ValidLevelId,
            ValidCourseId,
            ValidTeacherId,
            GroupFormat.Mixed,
            ValidRoomId,
            OnlinePlatform.GoogleMeet,
            10,
            ValidStartDate,
            ValidEndDate
        );

        group.RoomId.ShouldBe(ValidRoomId);
        group.Platform.ShouldBe(OnlinePlatform.GoogleMeet);
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingGroup_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Group(
                Guid.Empty,
                ValidCode,
                "Группа А-1",
                "Описание",
                ValidLevelId,
                ValidCourseId,
                ValidTeacherId,
                GroupFormat.Offline,
                ValidRoomId,
                null,
                10,
                ValidStartDate,
                ValidEndDate
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyLevelId_WhenCreatingGroup_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Group(
                ValidOrgId,
                ValidCode,
                "Группа А-1",
                "Описание",
                Guid.Empty,
                ValidCourseId,
                ValidTeacherId,
                GroupFormat.Offline,
                ValidRoomId,
                null,
                10,
                ValidStartDate,
                ValidEndDate
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyCourseId_WhenCreatingGroup_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Group(
                ValidOrgId,
                ValidCode,
                "Группа А-1",
                "Описание",
                ValidLevelId,
                Guid.Empty,
                ValidTeacherId,
                GroupFormat.Offline,
                ValidRoomId,
                null,
                10,
                ValidStartDate,
                ValidEndDate
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyTeacherId_WhenCreatingGroup_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Group(
                ValidOrgId,
                ValidCode,
                "Группа А-1",
                "Описание",
                ValidLevelId,
                ValidCourseId,
                Guid.Empty,
                GroupFormat.Offline,
                ValidRoomId,
                null,
                10,
                ValidStartDate,
                ValidEndDate
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenOfflineFormatWithoutRoom_WhenCreatingGroup_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Group(
                ValidOrgId,
                ValidCode,
                "Группа",
                "Описание",
                ValidLevelId,
                ValidCourseId,
                ValidTeacherId,
                GroupFormat.Offline,
                null, // нет кабинета
                null,
                10,
                ValidStartDate,
                ValidEndDate
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenOnlineFormatWithoutPlatform_WhenCreatingGroup_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Group(
                ValidOrgId,
                ValidCode,
                "Группа",
                "Описание",
                ValidLevelId,
                ValidCourseId,
                ValidTeacherId,
                GroupFormat.Online,
                null,
                null, // нет платформы
                10,
                ValidStartDate,
                ValidEndDate
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(51)]
    public void GivenInvalidCapacity_WhenCreatingGroup_ThenShouldThrowArgumentOutOfRangeException(
        int capacity
    )
    {
        var act = () =>
            new Group(
                ValidOrgId,
                ValidCode,
                "Группа",
                "Описание",
                ValidLevelId,
                ValidCourseId,
                ValidTeacherId,
                GroupFormat.Offline,
                ValidRoomId,
                null,
                capacity,
                ValidStartDate,
                ValidEndDate
            );

        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Test]
    public void GivenEndDateBeforeStartDate_WhenCreatingGroup_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Group(
                ValidOrgId,
                ValidCode,
                "Группа",
                "Описание",
                ValidLevelId,
                ValidCourseId,
                ValidTeacherId,
                GroupFormat.Offline,
                ValidRoomId,
                null,
                10,
                ValidStartDate,
                ValidStartDate.AddDays(-1)
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenActiveGroup_WhenUpdating_ThenShouldUpdateProperties()
    {
        var group = CreateValidOfflineGroup();
        var newEndDate = ValidEndDate.AddMonths(3);
        var newTeacherId = Guid.CreateVersion7();
        var newLevelId = Guid.CreateVersion7();

        group.Update(
            "Обновлённое название",
            "Обновлённое описание",
            newLevelId,
            ValidCourseId,
            newTeacherId,
            GroupFormat.Offline,
            ValidRoomId,
            null,
            20,
            newEndDate
        );

        group.Name.ShouldBe("Обновлённое название");
        group.Description.ShouldBe("Обновлённое описание");
        group.LevelId.ShouldBe(newLevelId);
        group.TeacherMemberId.ShouldBe(newTeacherId);
        group.Capacity.ShouldBe(20);
        group.EndDate.ShouldBe(newEndDate);
    }

    [Test]
    public void GivenArchivedGroup_WhenUpdating_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateValidOfflineGroup();
        group.Archive();

        var act = () =>
            group.Update(
                "Название",
                "Описание",
                ValidLevelId,
                ValidCourseId,
                ValidTeacherId,
                GroupFormat.Offline,
                ValidRoomId,
                null,
                10,
                ValidEndDate.AddMonths(1)
            );

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenActiveGroup_WhenChangingStatus_ThenStatusShouldBeUpdated()
    {
        var group = CreateValidOfflineGroup();

        group.ChangeStatus(GroupStatus.Active);

        group.Status.ShouldBe(GroupStatus.Active);
    }

    [Test]
    public void GivenArchivedGroup_WhenChangingStatus_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateValidOfflineGroup();
        group.Archive();

        var act = () => group.ChangeStatus(GroupStatus.Active);

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenActiveGroup_WhenAddingMember_ThenShouldAddToMembers()
    {
        var group = CreateValidOfflineGroup();
        var member = new GroupMember(
            ValidOrgId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupMemberRole.Student,
            ValidStartDate
        );

        group.AddMember(member);

        group.Members.ShouldHaveSingleItem();
        group.Members[0].ShouldBe(member);
    }

    [Test]
    public void GivenArchivedGroup_WhenAddingMember_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateValidOfflineGroup();
        group.Archive();
        var member = new GroupMember(
            ValidOrgId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupMemberRole.Student,
            ValidStartDate
        );

        var act = () => group.AddMember(member);

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenNullMember_WhenAddingMember_ThenShouldThrowArgumentNullException()
    {
        var group = CreateValidOfflineGroup();

        var act = () => group.AddMember(null!);

        act.ShouldThrow<ArgumentNullException>();
    }

    [Test]
    public void GivenActiveGroup_WhenArchiving_ThenStatusShouldBeArchived()
    {
        var group = CreateValidOfflineGroup();

        group.Archive();

        group.Status.ShouldBe(GroupStatus.Archived);
        group.IsDeleted.ShouldBeFalse();
    }

    [Test]
    public void GivenActiveGroup_WhenDeleting_ThenShouldMarkAsDeletedAndSetArchivedStatus()
    {
        var group = CreateValidOfflineGroup();

        group.Delete();

        group.IsDeleted.ShouldBeTrue();
        group.Status.ShouldBe(GroupStatus.Archived);
    }

    [Test]
    public void GivenNameWithLeadingSpaces_WhenCreatingGroup_ThenNameShouldBeTrimmed()
    {
        var group = new Group(
            ValidOrgId,
            ValidCode,
            "  Группа А-1  ",
            "  Описание  ",
            ValidLevelId,
            ValidCourseId,
            ValidTeacherId,
            GroupFormat.Offline,
            ValidRoomId,
            null,
            10,
            ValidStartDate,
            ValidEndDate
        );

        group.Name.ShouldBe("Группа А-1");
        group.Description.ShouldBe("Описание");
    }
}
