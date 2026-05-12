namespace Edvantix.Curriculum.UnitTests.Features.Modules.Reorder;

public sealed class ReorderModulesCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public ReorderModulesCommandHandlerTests()
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
    public async Task GivenExistingCourse_WhenHandling_ThenShouldReorderModules()
    {
        var course = CurriculumTestData.CreateCourse();
        var first = course.AddModule("First", null, 2);
        var second = course.AddModule("Second", null, 2);
        _repoMock
            .Setup(r => r.GetByIdForWriteAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(
            new ReorderModulesCommand(course.Id, [second.Id, first.Id]),
            CancellationToken.None
        );

        course.Modules.First(m => m.Id == second.Id).Position.ShouldBe((short)1);
        course.Modules.First(m => m.Id == first.Id).Position.ShouldBe((short)2);
    }

    [Test]
    public async Task GivenExistingCourse_WhenHandling_ThenShouldSaveEntities()
    {
        var course = CurriculumTestData.CreateCourse();
        var module = course.AddModule("Module", null, 2);
        _repoMock
            .Setup(r => r.GetByIdForWriteAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(
            new ReorderModulesCommand(course.Id, [module.Id]),
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
                    new ReorderModulesCommand(Guid.CreateVersion7(), []),
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
                .Handle(new ReorderModulesCommand(course.Id, []), CancellationToken.None)
                .AsTask()
        );
    }

    private ReorderModulesCommandHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);
}
