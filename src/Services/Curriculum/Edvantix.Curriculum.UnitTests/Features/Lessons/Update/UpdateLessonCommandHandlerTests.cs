namespace Edvantix.Curriculum.UnitTests.Features.Lessons.Update;

public sealed class UpdateLessonCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public UpdateLessonCommandHandlerTests()
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
    public async Task GivenExistingLesson_WhenHandling_ThenShouldUpdateLesson()
    {
        var course = CurriculumTestData.CreateCourseWithLesson(out _, out var lesson);
        _repoMock
            .Setup(r => r.GetByLessonIdAsync(lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(
            new UpdateLessonCommand(
                lesson.Id,
                "Updated Title",
                LessonType.Practice,
                60,
                ["New objective"]
            ),
            CancellationToken.None
        );

        lesson.Title.ShouldBe("Updated Title");
        lesson.Type.ShouldBe(LessonType.Practice);
        lesson.Minutes.ShouldBe((short)60);
        lesson.Objectives.ShouldContain("New objective");
    }

    [Test]
    public async Task GivenExistingLesson_WhenHandling_ThenShouldSaveEntities()
    {
        var course = CurriculumTestData.CreateCourseWithLesson(out _, out var lesson);
        _repoMock
            .Setup(r => r.GetByLessonIdAsync(lesson.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(
            new UpdateLessonCommand(lesson.Id, "Title", LessonType.Lecture, 45, []),
            CancellationToken.None
        );

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenMissingLesson_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(
                    new UpdateLessonCommand(
                        Guid.CreateVersion7(),
                        "Title",
                        LessonType.Lecture,
                        45,
                        []
                    ),
                    CancellationToken.None
                )
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
                .Handle(
                    new UpdateLessonCommand(lesson.Id, "Title", LessonType.Lecture, 45, []),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    private UpdateLessonCommandHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);
}
