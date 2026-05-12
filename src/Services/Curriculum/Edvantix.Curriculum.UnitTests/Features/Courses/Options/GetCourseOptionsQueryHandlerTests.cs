namespace Edvantix.Curriculum.UnitTests.Features.Courses.Options;

public sealed class GetCourseOptionsQueryHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();

    public GetCourseOptionsQueryHandlerTests()
    {
        _tenantContextMock
            .SetupGet(t => t.OrganizationId)
            .Returns(CurriculumTestData.OrganizationId);
    }

    [Test]
    public async Task GivenActiveCourses_WhenHandling_ThenShouldReturnMappedOptions()
    {
        var course = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<Specification<Course>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([course]);
        var handler = CreateHandler();

        var result = await handler.Handle(new GetCourseOptionsQuery(), CancellationToken.None);

        result.ShouldHaveSingleItem();
        result[0].Id.ShouldBe(course.Id);
        result[0].Code.ShouldBe(course.Code);
        result[0].Name.ShouldBe(course.Name);
        result[0].Level.ShouldBe(course.Level);
        result[0].Subject.ShouldBe(course.Subject);
    }

    [Test]
    public async Task GivenNoCourses_WhenHandling_ThenShouldReturnEmptyList()
    {
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<Specification<Course>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        var handler = CreateHandler();

        var result = await handler.Handle(
            new GetCourseOptionsQuery("nonexistent"),
            CancellationToken.None
        );

        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenMultipleCourses_WhenHandling_ThenShouldReturnAllMapped()
    {
        var course1 = CurriculumTestData.CreateCourse();
        var course2 = new Course(
            CurriculumTestData.OrganizationId,
            "MATH-GEN-A1",
            "Math General A1",
            CourseSubject.Math,
            "A1",
            8,
            CurriculumTestData.OwnerMemberId
        );
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<Specification<Course>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([course1, course2]);
        var handler = CreateHandler();

        var result = await handler.Handle(new GetCourseOptionsQuery(), CancellationToken.None);

        result.Count.ShouldBe(2);
    }

    private GetCourseOptionsQueryHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);
}
