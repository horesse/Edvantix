using Edvantix.Curriculum.Grpc.Services;
using Edvantix.Curriculum.Grpc.Services.Curriculum;
using Edvantix.Curriculum.UnitTests.Grpc.Context;
using Grpc.Core;

namespace Edvantix.Curriculum.UnitTests.Grpc.Services;

public sealed class CurriculumGrpcServiceTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();

    private CurriculumCatalogService CreateService() => new(_repoMock.Object);

    private static TestServerCallContext CreateContext() => new();

    // ─── GetCoursesForOrganization ─────────────────────────────────────────────

    [Test]
    public async Task GivenInvalidOrganizationId_WhenGetCoursesForOrganization_ThenShouldThrowRpcExceptionWithInvalidArgument()
    {
        var request = new GetCoursesForOrganizationRequest { OrganizationId = "not-a-guid" };

        var ex = await Should.ThrowAsync<RpcException>(() =>
            CreateService().GetCoursesForOrganization(request, CreateContext())
        );

        ex.StatusCode.ShouldBe(StatusCode.InvalidArgument);
    }

    [Test]
    public async Task GivenValidOrganizationIdWithCourses_WhenGetCoursesForOrganization_ThenShouldReturnMappedCourses()
    {
        var course = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<Specification<Course>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([course]);
        var request = new GetCoursesForOrganizationRequest
        {
            OrganizationId = CurriculumTestData.OrganizationId.ToString(),
        };

        var response = await CreateService().GetCoursesForOrganization(request, CreateContext());

        response.Courses.ShouldHaveSingleItem();
        response.Courses[0].Id.ShouldBe(course.Id.ToString());
        response.Courses[0].Code.ShouldBe(course.Code);
        response.Courses[0].Name.ShouldBe(course.Name);
        response.Courses[0].Level.ShouldBe(course.Level);
        response.Courses[0].Subject.ShouldBe(course.Subject.ToString());
    }

    [Test]
    public async Task GivenValidOrganizationIdWithNoCourses_WhenGetCoursesForOrganization_ThenShouldReturnEmptyResponse()
    {
        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<Specification<Course>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        var request = new GetCoursesForOrganizationRequest
        {
            OrganizationId = Guid.CreateVersion7().ToString(),
        };

        var response = await CreateService().GetCoursesForOrganization(request, CreateContext());

        response.Courses.ShouldBeEmpty();
    }

    // ─── GetCourseById ─────────────────────────────────────────────────────────

    [Test]
    public async Task GivenInvalidCourseId_WhenGetCourseById_ThenShouldThrowRpcExceptionWithInvalidArgument()
    {
        var request = new GetCourseByIdRequest { CourseId = "not-a-guid" };

        var ex = await Should.ThrowAsync<RpcException>(() =>
            CreateService().GetCourseById(request, CreateContext())
        );

        ex.StatusCode.ShouldBe(StatusCode.InvalidArgument);
    }

    [Test]
    public async Task GivenNonExistentCourseId_WhenGetCourseById_ThenShouldReturnFoundFalse()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course?)null);
        var request = new GetCourseByIdRequest { CourseId = Guid.CreateVersion7().ToString() };

        var response = await CreateService().GetCourseById(request, CreateContext());

        response.Found.ShouldBeFalse();
    }

    [Test]
    public async Task GivenExistingCourseId_WhenGetCourseById_ThenShouldReturnFoundTrueWithCourseInfo()
    {
        var course = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var request = new GetCourseByIdRequest { CourseId = course.Id.ToString() };

        var response = await CreateService().GetCourseById(request, CreateContext());

        response.Found.ShouldBeTrue();
        response.Course.Id.ShouldBe(course.Id.ToString());
        response.Course.OrganizationId.ShouldBe(course.OrganizationId.ToString());
        response.Course.Code.ShouldBe(course.Code);
        response.Course.Name.ShouldBe(course.Name);
        response.Course.Subject.ShouldBe(course.Subject.ToString());
        response.Course.Level.ShouldBe(course.Level);
        response.Course.DurationWeeks.ShouldBe(course.DurationWeeks);
        response.Course.Status.ShouldBe(course.Status.ToString());
    }

    // ─── GetCoursesByIds ───────────────────────────────────────────────────────

    [Test]
    public async Task GivenIds_WhenGetCoursesByIds_ThenReturnsMatching()
    {
        var course1 = CurriculumTestData.CreateCourse();
        var course2 = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([course1, course2]);
        var request = new GetCoursesByIdsRequest();
        request.CourseIds.Add(course1.Id.ToString());
        request.CourseIds.Add(course2.Id.ToString());

        var response = await CreateService().GetCoursesByIds(request, CreateContext());

        response.Courses.Count.ShouldBe(2);
        response.Courses.ShouldContain(c => c.Id == course1.Id.ToString() && c.Code == course1.Code && c.Name == course1.Name);
        response.Courses.ShouldContain(c => c.Id == course2.Id.ToString() && c.Code == course2.Code && c.Name == course2.Name);
    }

    [Test]
    public async Task GivenMoreThan200Ids_WhenGetCoursesByIds_ThenThrowsInvalidArgument()
    {
        var request = new GetCoursesByIdsRequest();
        request.CourseIds.AddRange(Enumerable.Range(0, 201).Select(_ => Guid.CreateVersion7().ToString()));

        var ex = await Should.ThrowAsync<RpcException>(() =>
            CreateService().GetCoursesByIds(request, CreateContext())
        );

        ex.StatusCode.ShouldBe(StatusCode.InvalidArgument);
    }

    [Test]
    public async Task GivenDeletedCourseInList_WhenGetCoursesByIds_ThenItIsExcluded()
    {
        var activeCourse = CurriculumTestData.CreateCourse();
        // Репозиторий применяет глобальный фильтр IsDeleted — возвращает только активный курс.
        _repoMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([activeCourse]);
        var deletedId = Guid.CreateVersion7();
        var request = new GetCoursesByIdsRequest();
        request.CourseIds.Add(activeCourse.Id.ToString());
        request.CourseIds.Add(deletedId.ToString());

        var response = await CreateService().GetCoursesByIds(request, CreateContext());

        response.Courses.ShouldHaveSingleItem();
        response.Courses[0].Id.ShouldBe(activeCourse.Id.ToString());
    }
}
