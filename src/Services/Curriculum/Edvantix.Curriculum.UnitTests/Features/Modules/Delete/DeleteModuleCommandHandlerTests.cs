namespace Edvantix.Curriculum.UnitTests.Features.Modules.Delete;

public sealed class DeleteModuleCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public DeleteModuleCommandHandlerTests()
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
    public async Task GivenExistingModule_WhenHandling_ThenShouldDeleteModule()
    {
        var course = CurriculumTestData.CreateCourseWithModule(out var module);
        _repoMock
            .Setup(r => r.GetByIdForWriteAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(
            new DeleteModuleCommand(course.Id, module.Id),
            CancellationToken.None
        );

        course.Modules.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenExistingModule_WhenHandling_ThenShouldSaveEntities()
    {
        var course = CurriculumTestData.CreateCourseWithModule(out var module);
        _repoMock
            .Setup(r => r.GetByIdForWriteAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(
            new DeleteModuleCommand(course.Id, module.Id),
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
                    new DeleteModuleCommand(Guid.CreateVersion7(), Guid.CreateVersion7()),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenCourseFromAnotherTenant_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var course = CurriculumTestData.CreateCourseWithModule(
            out var module,
            CurriculumTestData.OtherOrganizationId
        );
        _repoMock
            .Setup(r => r.GetByIdForWriteAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(new DeleteModuleCommand(course.Id, module.Id), CancellationToken.None)
                .AsTask()
        );
    }

    [Test]
    public async Task GivenMissingModule_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var course = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r => r.GetByIdForWriteAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(
                    new DeleteModuleCommand(course.Id, Guid.CreateVersion7()),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    private DeleteModuleCommandHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);
}
