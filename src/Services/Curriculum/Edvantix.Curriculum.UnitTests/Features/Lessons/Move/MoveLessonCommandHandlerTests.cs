namespace Edvantix.Curriculum.UnitTests.Features.Lessons.Move;

public sealed class MoveLessonCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public MoveLessonCommandHandlerTests()
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
    public async Task GivenTwoLessons_WhenMovingFirstToSecond_ThenPositionsShouldSwap()
    {
        var course = CurriculumTestData.CreateCourseWithLesson(out var module, out var lesson1);
        var lesson2 = course.AddLesson(module.Id, "Lesson 2", LessonType.Practice, 60, []);
        _repoMock
            .Setup(r => r.GetByLessonIdAsync(lesson1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(new MoveLessonCommand(lesson1.Id, 2), CancellationToken.None);

        module.Lessons[0].Id.ShouldBe(lesson2.Id);
        module.Lessons[0].Position.ShouldBe((short)1);
        module.Lessons[1].Id.ShouldBe(lesson1.Id);
        module.Lessons[1].Position.ShouldBe((short)2);
    }

    [Test]
    public async Task GivenExistingLesson_WhenHandling_ThenShouldSaveEntities()
    {
        var course = CurriculumTestData.CreateCourseWithLesson(out var module, out var lesson1);
        course.AddLesson(module.Id, "Lesson 2", LessonType.Practice, 60, []);
        _repoMock
            .Setup(r => r.GetByLessonIdAsync(lesson1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(new MoveLessonCommand(lesson1.Id, 2), CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenMissingLesson_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(new MoveLessonCommand(Guid.CreateVersion7(), 1), CancellationToken.None)
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
            handler
                .Handle(new MoveLessonCommand(lesson.Id, 1), CancellationToken.None)
                .AsTask()
        );
    }

    private MoveLessonCommandHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);
}
