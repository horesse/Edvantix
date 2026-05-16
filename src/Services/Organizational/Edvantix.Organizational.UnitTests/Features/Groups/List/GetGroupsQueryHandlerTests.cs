namespace Edvantix.Organizational.UnitTests.Features.Groups.List;

public sealed class GetGroupsQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Mock<IMapper<Group, GroupListItemDto>> _mapperMock = new();
    private readonly Mock<IProfileService> _profileServiceMock = new();
    private readonly Mock<ICurriculumService> _curriculumServiceMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetGroupsQueryHandler _handler;

    public GetGroupsQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _curriculumServiceMock
            .Setup(c =>
                c.GetCoursesByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, CourseRefDto>());
        _handler = new(
            _tenantMock.Object,
            _repoMock.Object,
            _mapperMock.Object,
            _profileServiceMock.Object,
            _curriculumServiceMock.Object
        );
    }

    [Test]
    public async Task GivenGroupsExist_WhenHandling_ThenShouldReturnPagedResult()
    {
        var group = CreateGroup();
        var dto = CreateDto(group.Id);
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 10);
        var teacherProfileId = Guid.CreateVersion7();

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([group]);
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);
        _repoMock
            .Setup(r =>
                r.GetTeacherMemberInfoAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Dictionary<Guid, OrganizationMember>
                {
                    [group.TeacherMemberId] = new(
                        _organizationId,
                        teacherProfileId,
                        Guid.CreateVersion7(),
                        new DateOnly(2025, 1, 1)
                    ),
                }
            );
        _repoMock
            .Setup(r =>
                r.GetRoomsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, Room>());
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);
        _profileServiceMock
            .Setup(p =>
                p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new GetProfilesResponse
                {
                    Profiles =
                    {
                        new GetProfileResponse
                        {
                            Id = teacherProfileId.ToString(),
                            FullName = "Иванов Иван Иванович",
                        },
                    },
                }
            );

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.TotalItems.ShouldBe(1);
        result[0].Teacher.FullName.ShouldBe("Иванов Иван Иванович");
    }

    [Test]
    public async Task GivenGroupsWithCourses_WhenHandling_ThenCourseNameIsPopulated()
    {
        var courseId = Guid.CreateVersion7();
        var group = CreateGroup(courseId: courseId);
        var dto = CreateDto(group.Id, courseId: courseId);
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 10);

        SetupEmptyGroupLists(group, dto);
        _curriculumServiceMock
            .Setup(c =>
                c.GetCoursesByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new Dictionary<Guid, CourseRefDto>
                {
                    [courseId] = new(courseId, "EN-GEN-B1", "Английский Общий B1"),
                }
            );

        var result = await _handler.Handle(query, CancellationToken.None);

        result[0].CourseCode.ShouldBe("EN-GEN-B1");
        result[0].CourseName.ShouldBe("Английский Общий B1");
        result[0].CourseId.ShouldBe(courseId);
    }

    [Test]
    public async Task GivenDeletedCourse_WhenHandling_ThenCourseNameIsFallback()
    {
        var courseId = Guid.CreateVersion7();
        var group = CreateGroup(courseId: courseId);
        var dto = CreateDto(group.Id, courseId: courseId);
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 10);

        SetupEmptyGroupLists(group, dto);

        // Curriculum не возвращает удалённый курс — CourseCode/CourseName остаются пустыми.
        _curriculumServiceMock
            .Setup(c =>
                c.GetCoursesByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, CourseRefDto>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result[0].CourseId.ShouldBe(courseId);
        result[0].CourseCode.ShouldBe(string.Empty);
        result[0].CourseName.ShouldBe(string.Empty);
    }

    [Test]
    public async Task GivenNoGroups_WhenHandling_ThenShouldReturnEmptyPagedResult()
    {
        var query = new GetGroupsQuery();

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.ShouldBeEmpty();
        result.TotalItems.ShouldBe(0);
        _profileServiceMock.Verify(
            p => p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenPageIndexBelowOne_WhenHandling_ThenShouldClampToOne()
    {
        var query = new GetGroupsQuery(PageIndex: -5, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageIndex.ShouldBe(1);
    }

    [Test]
    public async Task GivenPageSizeAbove100_WhenHandling_ThenShouldClampTo100()
    {
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 999);

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageSize.ShouldBe(100);
    }

    [Test]
    public async Task GivenGroupsWithNoTeacherProfiles_WhenHandling_ThenShouldNotCallProfileService()
    {
        var group = CreateGroup();
        var dto = CreateDto(group.Id);
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([group]);
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);
        _repoMock
            .Setup(r =>
                r.GetTeacherMemberInfoAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new Dictionary<Guid, OrganizationMember>());
        _repoMock
            .Setup(r =>
                r.GetRoomsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, Room>());
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result[0].Teacher.FullName.ShouldBe(string.Empty);
        _profileServiceMock.Verify(
            p => p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenGroupWithRoom_WhenHandling_ThenShouldEnrichRoomLabel()
    {
        var roomId = Guid.CreateVersion7();
        var group = CreateGroupWithRoom(roomId);
        var dto = CreateDto(group.Id);
        var room = new Room(_organizationId, "Каб. 205", 2, 30);
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([group]);
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);
        _repoMock
            .Setup(r =>
                r.GetTeacherMemberInfoAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new Dictionary<Guid, OrganizationMember>());
        _repoMock
            .Setup(r =>
                r.GetRoomsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, Room> { [roomId] = room });
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result[0].RoomLabel.ShouldBe("Каб. 205");
    }

    /// <summary>
    /// Настраивает репозиторий и маппер для сценария с одной группой без учителя, кабинета
    /// и данных о профиле. Использовать в тестах, ориентированных на course-enrichment.
    /// </summary>
    private void SetupEmptyGroupLists(Group group, GroupListItemDto dto)
    {
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([group]);
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);
        _repoMock
            .Setup(r =>
                r.GetTeacherMemberInfoAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new Dictionary<Guid, OrganizationMember>());
        _repoMock
            .Setup(r =>
                r.GetRoomsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, Room>());
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);
    }

    private Group CreateGroupWithRoom(Guid roomId) =>
        new(
            _organizationId,
            GroupCode.From("B1-02"),
            "Английский B1 очный",
            "Описание",
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupFormat.Offline,
            roomId,
            null,
            10,
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 6, 30)
        );

    private Group CreateGroup(Guid? courseId = null) =>
        new(
            _organizationId,
            GroupCode.From("B1-01"),
            "Английский B1",
            "Описание",
            Guid.CreateVersion7(),
            courseId ?? Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            GroupFormat.Online,
            null,
            OnlinePlatform.Zoom,
            10,
            new DateOnly(2025, 9, 1),
            new DateOnly(2026, 6, 30)
        );

    private static GroupListItemDto CreateDto(Guid id, Guid? courseId = null) =>
        new(
            id,
            "B1-01",
            "Английский B1",
            LevelId: Guid.CreateVersion7(),
            LevelCode: "B1",
            LevelName: "B1 — Средний",
            LevelTone: LevelTone.Blue,
            CourseId: courseId ?? Guid.CreateVersion7(),
            CourseCode: string.Empty,
            CourseName: string.Empty,
            Teacher: new TeacherDto(Guid.CreateVersion7(), string.Empty, string.Empty, null),
            RoomId: null,
            RoomLabel: null,
            Format: GroupFormat.Online,
            Platform: OnlinePlatform.Zoom,
            ScheduleSummary: null,
            Capacity: 10,
            MemberCount: 0,
            Status: GroupStatus.Recruiting,
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30)
        );
}
