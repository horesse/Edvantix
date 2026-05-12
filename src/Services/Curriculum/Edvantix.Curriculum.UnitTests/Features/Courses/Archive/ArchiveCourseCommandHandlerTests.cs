namespace Edvantix.Curriculum.UnitTests.Features.Courses.Archive;

public sealed class ArchiveCourseCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public ArchiveCourseCommandHandlerTests()
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
    public async Task GivenExistingCourse_WhenHandling_ThenShouldArchiveCourse()
    {
        var course = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(new ArchiveCourseCommand(course.Id), CancellationToken.None);

        course.Status.ShouldBe(CourseStatus.Archived);
    }

    [Test]
    public async Task GivenExistingCourse_WhenHandling_ThenShouldRegisterArchivedDomainEvent()
    {
        var course = CurriculumTestData.CreateCourse();
        course.ClearDomainEvents();
        _repoMock
            .Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(new ArchiveCourseCommand(course.Id), CancellationToken.None);

        var @event = course.DomainEvents.Single().ShouldBeOfType<CourseArchivedDomainEvent>();
        @event.CourseId.ShouldBe(course.Id);
        @event.OrganizationId.ShouldBe(CurriculumTestData.OrganizationId);
    }

    [Test]
    public async Task GivenMissingCourse_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(new ArchiveCourseCommand(Guid.CreateVersion7()), CancellationToken.None)
                .AsTask()
        );
    }

    [Test]
    public async Task GivenCourseFromAnotherTenant_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var course = CurriculumTestData.CreateCourse(CurriculumTestData.OtherOrganizationId);
        _repoMock
            .Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler.Handle(new ArchiveCourseCommand(course.Id), CancellationToken.None).AsTask()
        );
    }

    private ArchiveCourseCommandHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);
}
