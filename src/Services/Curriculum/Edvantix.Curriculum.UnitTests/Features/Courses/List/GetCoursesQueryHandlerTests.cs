namespace Edvantix.Curriculum.UnitTests.Features.Courses.List;

public sealed class GetCoursesQueryHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IMapper<Course, CourseDto>> _mapperMock = new();

    public GetCoursesQueryHandlerTests()
    {
        _tenantContextMock
            .SetupGet(t => t.OrganizationId)
            .Returns(CurriculumTestData.OrganizationId);
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<Specification<Course>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<Specification<Course>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);
    }

    [Test]
    public async Task GivenValidQuery_WhenHandling_ThenShouldReturnPagedResult()
    {
        var course = CurriculumTestData.CreateCourse();
        var dto = BuildDto(course.Id);
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<Specification<Course>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([course]);
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<Specification<Course>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map(course)).Returns(dto);
        var handler = CreateHandler();

        var result = await handler.Handle(new GetCoursesQuery(), CancellationToken.None);

        result.ShouldHaveSingleItem();
        result.TotalItems.ShouldBe(1);
    }

    [Test]
    public async Task GivenPageIndexBelowMinimum_WhenHandling_ThenShouldClampToOne()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new GetCoursesQuery(PageIndex: -5),
            CancellationToken.None
        );

        result.PageIndex.ShouldBe(1);
    }

    [Test]
    public async Task GivenPageSizeAboveMaximum_WhenHandling_ThenShouldClampTo100()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new GetCoursesQuery(PageSize: 500),
            CancellationToken.None
        );

        result.PageSize.ShouldBe(100);
    }

    [Test]
    public async Task GivenPageSizeBelowMinimum_WhenHandling_ThenShouldClampToOne()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new GetCoursesQuery(PageSize: 0),
            CancellationToken.None
        );

        result.PageSize.ShouldBe(1);
    }

    [Test]
    public async Task GivenEmptyRepository_WhenHandling_ThenShouldReturnEmptyPagedResult()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(new GetCoursesQuery(), CancellationToken.None);

        result.ShouldBeEmpty();
        result.TotalItems.ShouldBe(0);
    }

    private GetCoursesQueryHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object, _mapperMock.Object);

    private static CourseDto BuildDto(Guid id) =>
        new(id, "EN", "English", CourseSubject.English, "B1", 12, null, CourseStatus.Draft, 0);
}
