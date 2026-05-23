using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Groups.Features.Directories.Subjects.Restore;

namespace Edvantix.Groups.UnitTests.Features.Directories.Subjects.Restore;

public sealed class RestoreSubjectCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ISubjectRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly RestoreSubjectCommandHandler _handler;

    public RestoreSubjectCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenArchivedSubject_WhenRestoring_ThenRestoresAndSaves()
    {
        var subject = CreateArchivedSubject(_organizationId);
        SetupSubject(subject);
        SetupSave();

        await _handler.Handle(new RestoreSubjectCommand(subject.Id), CancellationToken.None);

        subject.IsArchived.ShouldBeFalse();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenActiveSubject_WhenRestoring_ThenNoOpButSaves()
    {
        var subject = CreateSubject(_organizationId);
        SetupSubject(subject);
        SetupSave();

        await _handler.Handle(new RestoreSubjectCommand(subject.Id), CancellationToken.None);

        subject.IsArchived.ShouldBeFalse();
    }

    [Test]
    public async Task GivenSubjectNotFound_WhenRestoring_ThenThrowsNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subject?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new RestoreSubjectCommand(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenSubjectFromOtherOrg_WhenRestoring_ThenThrowsNotFoundException()
    {
        var subject = CreateArchivedSubject(Guid.CreateVersion7());
        SetupSubject(subject);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new RestoreSubjectCommand(subject.Id), CancellationToken.None).AsTask()
        );
    }

    private void SetupSubject(Subject subject) =>
        _repoMock
            .Setup(r => r.GetByIdAsync(subject.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);

    private void SetupSave() =>
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

    private static Subject CreateSubject(Guid orgId) =>
        new(orgId, "Математика", SubjectCode.From("MATH"), "#6366F1", null);

    private static Subject CreateArchivedSubject(Guid orgId)
    {
        var subject = CreateSubject(orgId);
        subject.Archive(Guid.Empty);
        return subject;
    }
}
