namespace Edvantix.Curriculum.UnitTests.Features.Courses.Publish;

public sealed class PublishCourseCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public PublishCourseCommandHandlerTests()
    {
        _tenantContextMock
            .SetupGet(t => t.OrganizationId)
            .Returns(CurriculumTestData.OrganizationId);
        _repoMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    [Test]
    public async Task GivenExistingCourse_WhenHandling_ThenShouldPublishCourse()
    {
        var course = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r => r.GetByIdForWriteAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(new PublishCourseCommand(course.Id), CancellationToken.None);

        course.Status.ShouldBe(CourseStatus.Active);
    }

    [Test]
    public async Task GivenExistingCourse_WhenHandling_ThenShouldSaveEntities()
    {
        var course = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r => r.GetByIdForWriteAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(new PublishCourseCommand(course.Id), CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenMissingCourse_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var courseId = Guid.CreateVersion7();
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler.Handle(new PublishCourseCommand(courseId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenCourseFromAnotherTenant_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var course = CurriculumTestData.CreateCourse(CurriculumTestData.OtherOrganizationId);
        _repoMock
            .Setup(r => r.GetByIdForWriteAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler.Handle(new PublishCourseCommand(course.Id), CancellationToken.None).AsTask()
        );
    }

    private PublishCourseCommandHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);
}
