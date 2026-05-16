namespace Edvantix.Organizational.UnitTests.Features.Groups.Get;

public sealed class GetGroupByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Mock<IMapper<Group, GroupDetailDto>> _mapperMock = new();
    private readonly Mock<IProfileService> _profileServiceMock = new();
    private readonly Mock<ICurriculumService> _curriculumServiceMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetGroupByIdQueryHandler _handler;

    public GetGroupByIdQueryHandlerTests()
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
    public async Task GivenExistingGroup_WhenHandling_ThenShouldReturnDtoEnrichedWithTeacherName()
    {
        var group = CreateGroup();
        var teacherProfileId = Guid.CreateVersion7();
        var dto = CreateDto(group.Id);

        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);
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
        _repoMock
            .Setup(r =>
                r.GetRoomsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, Room>());

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.Teacher.FullName.ShouldBe("Иванов Иван Иванович");
        result.Teacher.MemberId.ShouldBe(group.TeacherMemberId);
    }

    [Test]
    public async Task GivenGroupWithCourse_WhenHandling_ThenShouldReturnDtoEnrichedWithCourseName()
    {
        var courseId = Guid.CreateVersion7();
        var group = CreateGroup(courseId: courseId);
        var dto = CreateDto(group.Id, courseId: courseId);

        SetupBaseGroupMocks(group, dto);
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

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.CourseId.ShouldBe(courseId);
        result.CourseCode.ShouldBe("EN-GEN-B1");
        result.CourseName.ShouldBe("Английский Общий B1");
    }

    [Test]
    public async Task GivenGroupWithRoom_WhenHandling_ThenShouldReturnDtoEnrichedWithRoomLabel()
    {
        var roomId = Guid.CreateVersion7();
        var group = CreateGroupWithRoom(roomId);
        var room = new Room(_organizationId, "Каб. 101", 1, 20);
        var dto = CreateDto(group.Id) with { RoomId = roomId };

        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);
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

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.RoomLabel.ShouldBe("Каб. 101");
    }

    [Test]
    public async Task GivenGroupNotFound_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var act = async () =>
            await _handler.Handle(new GetGroupByIdQuery(id), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenGroupOfDifferentOrganization_WhenHandling_ThenShouldThrowForbiddenException()
    {
        var group = CreateGroup(organizationId: Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var act = async () =>
            await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        await act.ShouldThrowAsync<ForbiddenException>();
    }

    [Test]
    public async Task GivenTeacherProfileNotFound_WhenHandling_ThenShouldReturnDtoWithEmptyTeacherName()
    {
        var group = CreateGroup();
        var dto = CreateDto(group.Id);

        SetupBaseGroupMocks(group, dto);
        _repoMock
            .Setup(r =>
                r.GetTeacherMemberInfoAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new Dictionary<Guid, OrganizationMember>());

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.Teacher.FullName.ShouldBe(string.Empty);
        _profileServiceMock.Verify(
            p => p.GetProfilesByIdsAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenRoomNotFound_WhenHandling_ThenShouldReturnDtoWithNullRoomLabel()
    {
        var roomId = Guid.CreateVersion7();
        var group = CreateGroupWithRoom(roomId);
        var dto = CreateDto(group.Id) with { RoomId = roomId };

        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);
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

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.RoomLabel.ShouldBeNull();
    }

    [Test]
    public async Task GivenGroupWithNullRoom_WhenHandling_ThenShouldNotCallGetRoomsByIdsAsync()
    {
        var group = CreateGroup();
        var dto = CreateDto(group.Id);

        SetupBaseGroupMocks(group, dto);

        await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        _repoMock.Verify(
            r => r.GetRoomsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenCourseNotFoundInCurriculumService_WhenHandling_ThenCourseFieldsRemainEmpty()
    {
        var courseId = Guid.CreateVersion7();
        var group = CreateGroup(courseId: courseId);
        var dto = CreateDto(group.Id, courseId: courseId);

        SetupBaseGroupMocks(group, dto);
        _curriculumServiceMock
            .Setup(c =>
                c.GetCoursesByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, CourseRefDto>());

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.CourseCode.ShouldBe(string.Empty);
        result.CourseName.ShouldBe(string.Empty);
    }

    [Test]
    public async Task GivenTeacherWithAvatarUrl_WhenHandling_ThenShouldReturnDtoWithAvatarUrl()
    {
        var group = CreateGroup();
        var teacherProfileId = Guid.CreateVersion7();
        var dto = CreateDto(group.Id);
        const string avatarUrl = "https://cdn.example.com/avatars/ivanov.jpg";

        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);
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
                            FullName = "Иванов Иван",
                            AvatarUrl = avatarUrl,
                        },
                    },
                }
            );
        _repoMock
            .Setup(r =>
                r.GetRoomsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, Room>());

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.Teacher.AvatarUrl.ShouldBe(avatarUrl);
    }

    private void SetupBaseGroupMocks(Group group, GroupDetailDto dto)
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);
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
    }

    private Group CreateGroup(Guid? organizationId = null, Guid? courseId = null) =>
        new(
            organizationId ?? _organizationId,
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

    private static GroupDetailDto CreateDto(Guid id, Guid? courseId = null) =>
        new(
            id,
            "B1-01",
            "Английский B1",
            "Описание",
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
            Schedule: null,
            UpcomingLessons: [],
            Capacity: 10,
            MemberCount: 0,
            Status: GroupStatus.Recruiting,
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30)
        );
}
