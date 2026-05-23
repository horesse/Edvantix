using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Groups.Features.Directories.Subjects.Update;

namespace Edvantix.Groups.UnitTests.Features.Directories.Subjects.Update;

public sealed class UpdateSubjectCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ISubjectRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly UpdateSubjectCommandHandler _handler;

    public UpdateSubjectCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingSubject_WhenUpdating_ThenUpdatesFieldsAndSaves()
    {
        var subject = CreateSubject(_organizationId);
        var command = BuildCommand(subject.Id);

        SetupSubject(subject);
        SetupNoDuplicates();
        SetupSave();

        await _handler.Handle(command, CancellationToken.None);

        subject.Name.ShouldBe(command.Name);
        subject.Code.Value.ShouldBe(command.Code);
        subject.Color.ShouldBe(command.Color.ToUpperInvariant());
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenSubjectNotFound_WhenUpdating_ThenThrowsNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subject?)null);

        var act = async () => await _handler.Handle(BuildCommand(id), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenSubjectFromOtherOrg_WhenUpdating_ThenThrowsNotFoundException()
    {
        var subject = CreateSubject(Guid.CreateVersion7());
        SetupSubject(subject);

        var act = async () =>
            await _handler.Handle(BuildCommand(subject.Id), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenDuplicateCode_WhenUpdating_ThenThrowsInvalidOperationException()
    {
        var subject = CreateSubject(_organizationId);
        var command = BuildCommand(subject.Id, code: "PHYS"); // different from subject's MATH

        SetupSubject(subject);

        _repoMock
            .Setup(r =>
                r.ExistsWithCodeAsync(
                    _organizationId,
                    It.IsAny<SubjectCode>(),
                    subject.Id,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        _repoMock
            .Setup(r =>
                r.AnyAsync(It.IsAny<ISpecification<Subject>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(false);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    private void SetupSubject(Subject subject) =>
        _repoMock
            .Setup(r => r.GetByIdAsync(subject.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);

    private void SetupNoDuplicates()
    {
        _repoMock
            .Setup(r =>
                r.ExistsWithCodeAsync(
                    _organizationId,
                    It.IsAny<SubjectCode>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        _repoMock
            .Setup(r =>
                r.AnyAsync(It.IsAny<ISpecification<Subject>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(false);
    }

    private void SetupSave() =>
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

    private static Subject CreateSubject(Guid orgId) =>
        new(orgId, "Математика", SubjectCode.From("MATH"), "#6366F1", null);

    private static UpdateSubjectCommand BuildCommand(Guid id, string code = "MATH") =>
        new(id, "Физика", code, "#EF4444", "Описание физики", 1);
}
