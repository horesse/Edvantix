using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Groups.Features.Directories.Subjects.Archive;

namespace Edvantix.Groups.UnitTests.Features.Directories.Subjects.Archive;

public sealed class ArchiveSubjectCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ISubjectRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly ArchiveSubjectCommandHandler _handler;

    public ArchiveSubjectCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenActiveSubject_WhenArchiving_ThenArchivesAndSaves()
    {
        var subject = CreateSubject(_organizationId);
        SetupSubject(subject);
        SetupSave();

        await _handler.Handle(new ArchiveSubjectCommand(subject.Id), CancellationToken.None);

        subject.IsArchived.ShouldBeTrue();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenAlreadyArchivedSubject_WhenArchiving_ThenNoOpButSaves()
    {
        var subject = CreateSubject(_organizationId);
        subject.Archive(Guid.Empty);
        SetupSubject(subject);
        SetupSave();

        await _handler.Handle(new ArchiveSubjectCommand(subject.Id), CancellationToken.None);

        subject.IsArchived.ShouldBeTrue();
    }

    [Test]
    public async Task GivenSubjectNotFound_WhenArchiving_ThenThrowsNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subject?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new ArchiveSubjectCommand(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenSubjectFromOtherOrg_WhenArchiving_ThenThrowsNotFoundException()
    {
        var subject = CreateSubject(Guid.CreateVersion7());
        SetupSubject(subject);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new ArchiveSubjectCommand(subject.Id), CancellationToken.None).AsTask()
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
}
