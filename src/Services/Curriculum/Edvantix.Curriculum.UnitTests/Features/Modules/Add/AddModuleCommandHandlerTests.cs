namespace Edvantix.Curriculum.UnitTests.Features.Modules.Add;

public sealed class AddModuleCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public AddModuleCommandHandlerTests()
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
    public async Task GivenExistingCourse_WhenHandling_ThenShouldAddModule()
    {
        var course = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r => r.GetByIdForWriteAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        var id = await handler.Handle(
            new AddModuleCommand(course.Id, "Module", "Summary", 2),
            CancellationToken.None
        );

        id.ShouldNotBe(Guid.Empty);
        course.Modules.ShouldHaveSingleItem();
        course.Modules[0].Name.ShouldBe("Module");
    }

    [Test]
    public async Task GivenExistingCourse_WhenHandling_ThenShouldSaveEntities()
    {
        var course = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r => r.GetByIdForWriteAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(
            new AddModuleCommand(course.Id, "Module", null, 2),
            CancellationToken.None
        );

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenMissingCourse_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(
                    new AddModuleCommand(Guid.CreateVersion7(), "Module", null, 2),
                    CancellationToken.None
                )
                .AsTask()
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
            handler
                .Handle(new AddModuleCommand(course.Id, "Module", null, 2), CancellationToken.None)
                .AsTask()
        );
    }

    private AddModuleCommandHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);
}
