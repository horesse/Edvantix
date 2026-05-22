using Edvantix.Groups.Domain.LessonTypeAggregate;
using Edvantix.Groups.Features.Directories.LessonTypes.Archive;

namespace Edvantix.Groups.UnitTests.Features.Directories.LessonTypes.Archive;

public sealed class ArchiveLessonTypeCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILessonTypeRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly ArchiveLessonTypeCommandHandler _handler;

    public ArchiveLessonTypeCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenActiveLessonType_WhenArchiving_ThenSavesChanges()
    {
        var (id, _) = SetupExistingActiveLessonType();
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(new ArchiveLessonTypeCommand(id), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenActiveLessonType_WhenArchiving_ThenIsArchivedTrue()
    {
        var (id, lessonType) = SetupExistingActiveLessonType();
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(new ArchiveLessonTypeCommand(id), CancellationToken.None);

        lessonType.IsArchived.ShouldBeTrue();
    }

    [Test]
    public async Task GivenAlreadyArchivedLessonType_WhenArchiving_ThenIdempotent()
    {
        var id = Guid.CreateVersion7();
        var lessonType = new LessonType(_organizationId, "Урок", "LESSON", 45, "#3B82F6", null);
        lessonType.Archive(Guid.Empty);

        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lessonType);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.NotThrowAsync(() =>
            _handler.Handle(new ArchiveLessonTypeCommand(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenNonExistentId_WhenArchiving_ThenThrowsNotFoundException()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LessonType?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new ArchiveLessonTypeCommand(Guid.CreateVersion7()), CancellationToken.None)
                .AsTask()
        );
    }

    private (Guid id, LessonType lessonType) SetupExistingActiveLessonType()
    {
        var id = Guid.CreateVersion7();
        var lessonType = new LessonType(_organizationId, "Урок", "LESSON", 45, "#3B82F6", null);

        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lessonType);

        return (id, lessonType);
    }
}
