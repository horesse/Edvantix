namespace Edvantix.Curriculum.UnitTests.Features.Courses.Create;

public sealed class CreateCourseCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _repoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public CreateCourseCommandHandlerTests()
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
    public async Task GivenValidCommand_WhenHandling_ThenShouldReturnCourseId()
    {
        var handler = CreateHandler();

        var id = await handler.Handle(BuildValidCommand(), CancellationToken.None);

        id.ShouldNotBe(Guid.Empty);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldAddCourseToRepository()
    {
        var handler = CreateHandler();

        await handler.Handle(BuildValidCommand(), CancellationToken.None);

        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldSaveEntities()
    {
        var handler = CreateHandler();

        await handler.Handle(BuildValidCommand(), CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenCourseShouldHaveCorrectProperties()
    {
        Course? captured = null;
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
            .Callback<Course, CancellationToken>((course, _) => captured = course);
        var command = BuildValidCommand();
        var handler = CreateHandler();

        await handler.Handle(command, CancellationToken.None);

        captured.ShouldNotBeNull();
        captured.OrganizationId.ShouldBe(CurriculumTestData.OrganizationId);
        captured.Code.ShouldBe(command.Code);
        captured.Name.ShouldBe(command.Name);
        captured.Subject.ShouldBe(command.Subject);
        captured.Level.ShouldBe(command.Level);
        captured.DurationWeeks.ShouldBe(command.DurationWeeks);
        captured.OwnerMemberId.ShouldBe(command.OwnerMemberId);
        captured.Description.ShouldBe(command.Description);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenCourseShouldRegisterCreatedDomainEvent()
    {
        Course? captured = null;
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
            .Callback<Course, CancellationToken>((course, _) => captured = course);
        var handler = CreateHandler();

        await handler.Handle(BuildValidCommand(), CancellationToken.None);

        captured.ShouldNotBeNull();
        var @event = captured.DomainEvents.Single().ShouldBeOfType<CourseCreatedDomainEvent>();
        @event.OrganizationId.ShouldBe(CurriculumTestData.OrganizationId);
        @event.OwnerMemberId.ShouldBe(CurriculumTestData.OwnerMemberId);
    }

    private CreateCourseCommandHandler CreateHandler() =>
        new(_tenantContextMock.Object, _repoMock.Object);

    private static CreateCourseCommand BuildValidCommand() =>
        new(
            "EN-GEN-B1",
            "English General B1",
            CourseSubject.English,
            "B1",
            DurationWeeks: 12,
            CurriculumTestData.OwnerMemberId,
            "General English course"
        );
}
