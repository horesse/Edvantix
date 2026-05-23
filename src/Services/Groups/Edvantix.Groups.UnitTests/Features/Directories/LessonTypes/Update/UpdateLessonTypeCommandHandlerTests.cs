using Edvantix.Groups.Domain.LessonTypeAggregate;
using Edvantix.Groups.Features.Directories.LessonTypes.Update;

namespace Edvantix.Groups.UnitTests.Features.Directories.LessonTypes.Update;

public sealed class UpdateLessonTypeCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILessonTypeRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly UpdateLessonTypeCommandHandler _handler;

    public UpdateLessonTypeCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingLessonType_WhenUpdating_ThenSavesChanges()
    {
        var (id, lessonType) = SetupExistingLessonType();
        SetupRepoPersist();

        await _handler.Handle(BuildCommand(id), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenExistingLessonType_WhenUpdating_ThenReturnsUpdatedDto()
    {
        var (id, _) = SetupExistingLessonType();
        SetupRepoPersist();
        var command = BuildCommand(id);

        var dto = await _handler.Handle(command, CancellationToken.None);

        dto.ShouldNotBeNull();
        dto.Name.ShouldBe(command.Name);
        dto.Code.ShouldBe(command.Code.ToUpperInvariant());
    }

    [Test]
    public async Task GivenNonExistentId_WhenUpdating_ThenThrowsNotFoundException()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LessonType?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(BuildCommand(Guid.CreateVersion7()), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenLessonTypeBelongingToOtherOrg_WhenUpdating_ThenThrowsNotFoundException()
    {
        var id = Guid.CreateVersion7();
        var otherOrgId = Guid.CreateVersion7();
        var lessonType = new LessonType(otherOrgId, "Урок", "LESSON", 45, "#3B82F6", null);

        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lessonType);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(BuildCommand(id), CancellationToken.None).AsTask()
        );
    }

    private (Guid id, LessonType lessonType) SetupExistingLessonType()
    {
        var id = Guid.CreateVersion7();
        var lessonType = new LessonType(_organizationId, "Старое имя", "OLD", 30, "#000000", null);

        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lessonType);

        return (id, lessonType);
    }

    private void SetupRepoPersist() =>
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

    private UpdateLessonTypeCommand BuildCommand(Guid id) =>
        new(id, _organizationId, "Консультация", "CONSULT", 60, "#EF4444", "MessageSquare");
}
