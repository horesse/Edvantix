namespace Edvantix.Groups.UnitTests.Features.Groups.List;

public sealed class GetGroupsQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Mock<IMapper<Group, GroupListItemDto>> _mapperMock = new();
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
            _curriculumServiceMock.Object
        );
    }

    [Test]
    public async Task GivenGroupsExist_WhenHandling_ThenShouldReturnPagedResult()
    {
        var group = CreateGroup();
        var dto = CreateDto(group.Id);
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 10);

        SetupGroupList(group, dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.TotalItems.ShouldBe(1);
    }

    [Test]
    public async Task GivenGroupsWithCourses_WhenHandling_ThenCourseNameIsPopulated()
    {
        var courseId = Guid.CreateVersion7();
        var group = CreateGroup(courseId: courseId);
        var dto = CreateDto(group.Id, courseId: courseId);
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 10);

        SetupGroupList(group, dto);
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
    }

    [Test]
    public async Task GivenDeletedCourse_WhenHandling_ThenCourseFieldsAreNull()
    {
        var courseId = Guid.CreateVersion7();
        var group = CreateGroup(courseId: courseId);
        var dto = CreateDto(group.Id, courseId: courseId);
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 10);

        SetupGroupList(group, dto);

        // Curriculum не возвращает удалённый курс — CourseCode/CourseName остаются null.
        _curriculumServiceMock
            .Setup(c =>
                c.GetCoursesByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, CourseRefDto>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result[0].CourseCode.ShouldBeNull();
        result[0].CourseName.ShouldBeNull();
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
    public async Task GivenPageSizeBelowOne_WhenHandling_ThenShouldClampToOne()
    {
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 0);

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

        result.PageSize.ShouldBe(1);
    }

    [Test]
    public async Task GivenMultipleGroupsWithSameCourse_WhenHandling_ThenBothDtosShouldBeEnrichedWithCourseData()
    {
        var courseId = Guid.CreateVersion7();
        var group1 = CreateGroup(courseId: courseId);
        var group2 = CreateGroup(courseId: courseId);
        var dto1 = CreateDto(group1.Id, courseId: courseId);
        var dto2 = CreateDto(group2.Id, courseId: courseId);
        var query = new GetGroupsQuery(PageIndex: 1, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([group1, group2]);
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Group>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(2);
        _mapperMock.Setup(m => m.Map(group1)).Returns(dto1);
        _mapperMock.Setup(m => m.Map(group2)).Returns(dto2);
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
        result[1].CourseCode.ShouldBe("EN-GEN-B1");
        result[1].CourseName.ShouldBe("Английский Общий B1");
    }

    private void SetupGroupList(Group group, GroupListItemDto dto)
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
        _mapperMock.Setup(m => m.Map(group)).Returns(dto);
    }

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
            GroupStatus.Recruiting,
            GroupFormat.Online,
            10,
            LevelId: Guid.CreateVersion7(),
            LevelCode: "B1",
            LevelName: "B1 — Средний",
            StartDate: new DateOnly(2025, 9, 1),
            EndDate: new DateOnly(2026, 6, 30),
            CourseCode: null,
            CourseName: null
        );
}
