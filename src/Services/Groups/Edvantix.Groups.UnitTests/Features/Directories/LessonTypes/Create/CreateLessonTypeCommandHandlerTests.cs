using Edvantix.Groups.Domain.LessonTypeAggregate;
using Edvantix.Groups.Features.Directories.LessonTypes.Create;

namespace Edvantix.Groups.UnitTests.Features.Directories.LessonTypes.Create;

public sealed class CreateLessonTypeCommandHandlerTests
{
    private readonly Mock<ILessonTypeRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly CreateLessonTypeCommandHandler _handler;

    public CreateLessonTypeCommandHandlerTests()
    {
        _handler = new(_repoMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenAddsLessonType()
    {
        SetupRepoPersist();

        await _handler.Handle(BuildCommand(), CancellationToken.None);

        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<LessonType>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenSavesChanges()
    {
        SetupRepoPersist();

        await _handler.Handle(BuildCommand(), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenLessonTypeBelongsToOrganization()
    {
        LessonType? captured = null;

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<LessonType>(), It.IsAny<CancellationToken>()))
            .Callback<LessonType, CancellationToken>((lt, _) => captured = lt)
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(BuildCommand(), CancellationToken.None);

        captured.ShouldNotBeNull();
        captured.OrganizationId.ShouldBe(_organizationId);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenReturnsDto()
    {
        SetupRepoPersist();

        var dto = await _handler.Handle(BuildCommand(), CancellationToken.None);

        dto.ShouldNotBeNull();
        dto.Name.ShouldBe("Урок");
        dto.Code.ShouldBe("LESSON");
        dto.DefaultDurationMinutes.ShouldBe(45);
        dto.Color.ShouldBe("#3B82F6");
    }

    private void SetupRepoPersist()
    {
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<LessonType>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private CreateLessonTypeCommand BuildCommand() =>
        new(_organizationId, "Урок", "LESSON", 45, "#3B82F6", "CalendarDays");
}
