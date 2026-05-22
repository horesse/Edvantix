using Edvantix.Groups.Domain.LessonTypeAggregate;
using Edvantix.Groups.Features.Directories.LessonTypes.Restore;

namespace Edvantix.Groups.UnitTests.Features.Directories.LessonTypes.Restore;

public sealed class RestoreLessonTypeCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILessonTypeRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly RestoreLessonTypeCommandHandler _handler;

    public RestoreLessonTypeCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenArchivedLessonType_WhenRestoring_ThenIsArchivedFalse()
    {
        var (id, lessonType) = SetupArchivedLessonType();
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(new RestoreLessonTypeCommand(id), CancellationToken.None);

        lessonType.IsArchived.ShouldBeFalse();
    }

    [Test]
    public async Task GivenActiveLessonType_WhenRestoring_ThenIdempotent()
    {
        var id = Guid.CreateVersion7();
        var lessonType = new LessonType(_organizationId, "Урок", "LESSON", 45, "#3B82F6", null);

        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lessonType);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.NotThrowAsync(() =>
            _handler.Handle(new RestoreLessonTypeCommand(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenNonExistentId_WhenRestoring_ThenThrowsNotFoundException()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LessonType?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new RestoreLessonTypeCommand(Guid.CreateVersion7()), CancellationToken.None)
                .AsTask()
        );
    }

    private (Guid id, LessonType lessonType) SetupArchivedLessonType()
    {
        var id = Guid.CreateVersion7();
        var lessonType = new LessonType(_organizationId, "Урок", "LESSON", 45, "#3B82F6", null);
        lessonType.Archive(Guid.Empty);

        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lessonType);

        return (id, lessonType);
    }
}
