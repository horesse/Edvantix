namespace Edvantix.Curriculum.UnitTests.Features.Lessons.Add;

public sealed class AddLessonCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public AddLessonCommandHandlerTests()
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
    public async Task GivenExistingModule_WhenHandling_ThenShouldAddLesson()
    {
        var course = CurriculumTestData.CreateCourseWithModule(out var module);
        _repoMock
            .Setup(r => r.GetByModuleIdAsync(module.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        var id = await handler.Handle(
            new AddLessonCommand(module.Id, "Lesson", LessonType.Lecture, 45, ["Objective"]),
            CancellationToken.None
        );

        id.ShouldNotBe(Guid.Empty);
        module.Lessons.ShouldHaveSingleItem();
        module.Lessons[0].Title.ShouldBe("Lesson");
    }

    [Test]
    public async Task GivenExistingModule_WhenHandling_ThenShouldSaveEntities()
    {
        var course = CurriculumTestData.CreateCourseWithModule(out var module);
        _repoMock
            .Setup(r => r.GetByModuleIdAsync(module.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(
            new AddLessonCommand(module.Id, "Lesson", LessonType.Lecture, 45, []),
            CancellationToken.None
        );

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenMissingModule_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(
                    new AddLessonCommand(
                        Guid.CreateVersion7(),
                        "Lesson",
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
    public async Task GivenModuleFromAnotherTenant_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var course = CurriculumTestData.CreateCourseWithModule(
            out var module,
            CurriculumTestData.OtherOrganizationId
        );
        _repoMock
            .Setup(r => r.GetByModuleIdAsync(module.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(
                    new AddLessonCommand(module.Id, "Lesson", LessonType.Lecture, 45, []),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    private AddLessonCommandHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);
}
