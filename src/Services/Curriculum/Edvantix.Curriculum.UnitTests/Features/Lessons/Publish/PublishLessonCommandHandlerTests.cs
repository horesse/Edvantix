namespace Edvantix.Curriculum.UnitTests.Features.Lessons.Publish;

public sealed class PublishLessonCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public PublishLessonCommandHandlerTests()
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
    public async Task GivenExistingLesson_WhenHandling_ThenShouldPublishLesson()
    {
        var course = CurriculumTestData.CreateCourseWithLesson(out _, out var lesson);
        _repoMock
            .Setup(r => r.GetByLessonIdAsync(lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(new PublishLessonCommand(lesson.Id), CancellationToken.None);

        lesson.Status.ShouldBe(LessonStatus.Published);
    }

    [Test]
    public async Task GivenExistingLesson_WhenHandling_ThenShouldRegisterLessonPublishedDomainEvent()
    {
        var course = CurriculumTestData.CreateCourseWithLesson(out var module, out var lesson);
        course.ClearDomainEvents();
        _repoMock
            .Setup(r => r.GetByLessonIdAsync(lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(new PublishLessonCommand(lesson.Id), CancellationToken.None);

        var @event = course.DomainEvents.Single().ShouldBeOfType<LessonPublishedDomainEvent>();
        @event.CourseId.ShouldBe(course.Id);
        @event.ModuleId.ShouldBe(module.Id);
        @event.LessonId.ShouldBe(lesson.Id);
    }

    [Test]
    public async Task GivenMissingLesson_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(new PublishLessonCommand(Guid.CreateVersion7()), CancellationToken.None)
                .AsTask()
        );
    }

    [Test]
    public async Task GivenLessonFromAnotherTenant_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var course = CurriculumTestData.CreateCourseWithLesson(
            out _,
            out var lesson,
            CurriculumTestData.OtherOrganizationId
        );
        _repoMock
            .Setup(r => r.GetByLessonIdAsync(lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler.Handle(new PublishLessonCommand(lesson.Id), CancellationToken.None).AsTask()
        );
    }

    private PublishLessonCommandHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);
}
