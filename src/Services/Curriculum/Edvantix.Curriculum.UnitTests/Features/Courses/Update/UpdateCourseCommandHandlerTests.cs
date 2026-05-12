namespace Edvantix.Curriculum.UnitTests.Features.Courses.Update;

public sealed class UpdateCourseCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public UpdateCourseCommandHandlerTests()
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
    public async Task GivenExistingCourse_WhenHandling_ThenShouldUpdateCourse()
    {
        var course = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var command = new UpdateCourseCommand(
            course.Id,
            "Updated",
            "New description",
            "B2",
            16,
            "UPD"
        );
        var handler = CreateHandler();

        await handler.Handle(command, CancellationToken.None);

        course.Name.ShouldBe(command.Name);
        course.Description.ShouldBe(command.Description);
        course.Level.ShouldBe(command.Level);
        course.DurationWeeks.ShouldBe(command.DurationWeeks);
        course.CoverInitials.ShouldBe(command.CoverInitials);
    }

    [Test]
    public async Task GivenExistingCourse_WhenHandling_ThenShouldSaveEntities()
    {
        var course = CurriculumTestData.CreateCourse();
        _repoMock
            .Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await handler.Handle(
            new UpdateCourseCommand(course.Id, "Updated", null, "B2", 16),
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
                    new UpdateCourseCommand(Guid.CreateVersion7(), "Updated", null, "B2", 16),
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
            .Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        var handler = CreateHandler();

        await Should.ThrowAsync<NotFoundException>(() =>
            handler
                .Handle(
                    new UpdateCourseCommand(course.Id, "Updated", null, "B2", 16),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    private UpdateCourseCommandHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);
}
