namespace Edvantix.Curriculum.UnitTests.Features.Courses.Get;

public sealed class GetCourseByIdQueryHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IMapper<Course, CourseDetailDto>> _mapperMock = new();

    public GetCourseByIdQueryHandlerTests()
    {
        _tenantContextMock
            .SetupGet(t => t.OrganizationId)
            .Returns(CurriculumTestData.OrganizationId);
    }

    [Test]
    public async Task GivenExistingCourse_WhenHandling_ThenShouldReturnDto()
    {
        var course = CurriculumTestData.CreateCourse();
        var dto = BuildDto(course.Id);
        _repoMock
            .Setup(r => r.GetByIdWithModulesAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _mapperMock.Setup(m => m.Map(course)).Returns(dto);
        var handler = CreateHandler();

        var result = await handler.Handle(
            new GetCourseByIdQuery(course.Id),
            CancellationToken.None
        );

        result.ShouldBe(dto);
    }

    [Test]
    public async Task GivenExistingCourse_WhenHandling_ThenShouldCallMapperWithCourse()
    {
        var course = CurriculumTestData.CreateCourse();
        var dto = BuildDto(course.Id);
        _repoMock
            .Setup(r => r.GetByIdWithModulesAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _mapperMock.Setup(m => m.Map(course)).Returns(dto);
        var handler = CreateHandler();

        await handler.Handle(new GetCourseByIdQuery(course.Id), CancellationToken.None);

        _mapperMock.Verify(m => m.Map(course), Times.Once);
    }

    [Test]
    public async Task GivenMissingCourse_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(new GetCourseByIdQuery(Guid.CreateVersion7()), CancellationToken.None)
                .AsTask()
        );
    }

    [Test]
    public async Task GivenCourseFromAnotherTenant_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var course = CurriculumTestData.CreateCourse(CurriculumTestData.OtherOrganizationId);
        _repoMock
            .Setup(r => r.GetByIdWithModulesAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler.Handle(new GetCourseByIdQuery(course.Id), CancellationToken.None).AsTask()
        );
    }

    private GetCourseByIdQueryHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object, _mapperMock.Object);

    private static CourseDetailDto BuildDto(Guid id) =>
        new(
            id,
            "EN",
            "English",
            CourseSubject.English,
            "B1",
            12,
            null,
            null,
            CourseStatus.Draft,
            Guid.CreateVersion7(),
            [],
            []
        );
}
