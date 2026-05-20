namespace Edvantix.Groups.UnitTests.Features.Groups.Get;

public sealed class GetGroupByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Mock<IMapper<Group, GroupDetailDto>> _mapperMock = new();
    private readonly Mock<ICurriculumService> _curriculumServiceMock = new();
    private readonly Mock<IScheduleService> _scheduleServiceMock = new();
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
        _scheduleServiceMock
            .Setup(s =>
                s.GetScheduleByGroupIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((ScheduleDetailDto?)null);
        _scheduleServiceMock
            .Setup(s =>
                s.GetUpcomingLessonsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);

        _handler = new(
            _tenantMock.Object,
            _repoMock.Object,
            _mapperMock.Object,
            _curriculumServiceMock.Object,
            _scheduleServiceMock.Object
        );
    }

    [Test]
    public async Task GivenExistingGroup_WhenHandling_ThenShouldReturnDto()
    {
        var group = CreateGroup();
        var dto = CreateDto(group.Id);

        SetupBaseGroupMocks(group, dto);

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.ShouldBe(dto);
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
    public async Task GivenCourseNotFoundInCurriculumService_WhenHandling_ThenCourseFieldsRemainNull()
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

        result.CourseCode.ShouldBeNull();
        result.CourseName.ShouldBeNull();
    }

    [Test]
    public async Task GivenGroupWithSchedule_WhenHandling_ThenShouldReturnDtoWithScheduleDetails()
    {
        var group = CreateGroup();
        var dto = CreateDto(group.Id);
        var scheduleId = Guid.CreateVersion7();
        var schedule = new ScheduleDetailDto(
            Id: scheduleId,
            Recurrence: "Weekly",
            BiweeklyParity: null,
            LessonDurationMinutes: 90,
            StartDate: new DateOnly(2025, 9, 1),
            EndMode: "Date",
            EndDate: new DateOnly(2026, 6, 30),
            LessonCount: null,
            SkipHolidays: false,
            Slots: [new ScheduleSlotDto(1, 1080), new ScheduleSlotDto(3, 1080)],
            Exceptions: [],
            SummaryText: "Пн / Ср · 18:00–19:30"
        );

        SetupBaseGroupMocks(group, dto);
        _scheduleServiceMock
            .Setup(s => s.GetScheduleByGroupIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.Schedule.ShouldNotBeNull();
        result.Schedule!.Id.ShouldBe(scheduleId);
        result.Schedule.SummaryText.ShouldBe("Пн / Ср · 18:00–19:30");
        result.Schedule.Slots.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenGroupWithNoScheduleInScheduleService_WhenHandling_ThenScheduleIsNull()
    {
        var group = CreateGroup();
        var dto = CreateDto(group.Id);

        SetupBaseGroupMocks(group, dto);
        _scheduleServiceMock
            .Setup(s => s.GetScheduleByGroupIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ScheduleDetailDto?)null);

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.Schedule.ShouldBeNull();
    }

    [Test]
    public async Task GivenGroupWithUpcomingLessons_WhenHandling_ThenShouldReturnUpcomingLessons()
    {
        var group = CreateGroup();
        var dto = CreateDto(group.Id);
        var lessons = new List<UpcomingLessonDto>
        {
            new(new DateOnly(2025, 9, 1), new TimeOnly(18, 0), new TimeOnly(19, 30)),
            new(new DateOnly(2025, 9, 3), new TimeOnly(18, 0), new TimeOnly(19, 30)),
        };

        SetupBaseGroupMocks(group, dto);
        _scheduleServiceMock
            .Setup(s => s.GetUpcomingLessonsAsync(group.Id, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lessons);

        var result = await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        result.UpcomingLessons.Count.ShouldBe(2);
        result.UpcomingLessons[0].Date.ShouldBe(new DateOnly(2025, 9, 1));
        result.UpcomingLessons[0].StartTime.ShouldBe(new TimeOnly(18, 0));
        result.UpcomingLessons[0].EndTime.ShouldBe(new TimeOnly(19, 30));
    }

    [Test]
    public async Task GivenGroup_WhenHandling_ThenScheduleServiceIsAlwaysCalled()
    {
        var group = CreateGroup();
        var dto = CreateDto(group.Id);

        SetupBaseGroupMocks(group, dto);

        await _handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        _scheduleServiceMock.Verify(
            s => s.GetScheduleByGroupIdAsync(group.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _scheduleServiceMock.Verify(
            s => s.GetUpcomingLessonsAsync(group.Id, 5, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private void SetupBaseGroupMocks(Group group, GroupDetailDto dto)
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(group.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);
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

    private static GroupDetailDto CreateDto(Guid id, Guid? courseId = null) =>
        new(
            id,
            "B1-01",
            "Английский B1",
            "Описание",
            GroupStatus.Recruiting,
            GroupFormat.Online,
            10,
            LevelId: Guid.CreateVersion7(),
            LevelCode: "B1",
            LevelName: "B1 — Средний",
            CourseId: courseId ?? Guid.CreateVersion7(),
            CourseCode: null,
            CourseName: null,
            TeacherMemberId: Guid.CreateVersion7(),
            Teacher: new TeacherDto(Guid.CreateVersion7(), string.Empty, null),
            RoomId: null,
            Platform: OnlinePlatform.Zoom,
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30),
            Schedule: null,
            UpcomingLessons: []
        );
}
