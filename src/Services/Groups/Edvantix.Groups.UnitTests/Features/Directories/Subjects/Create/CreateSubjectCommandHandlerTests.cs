using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Groups.Features.Directories.Subjects.Create;

namespace Edvantix.Groups.UnitTests.Features.Directories.Subjects.Create;

public sealed class CreateSubjectCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ISubjectRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly CreateSubjectCommandHandler _handler;

    public CreateSubjectCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenAddsSubject()
    {
        var command = BuildCommand();
        SetupNoDuplicates();
        SetupRepoPersist();

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<Subject>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenSavesChanges()
    {
        var command = BuildCommand();
        SetupNoDuplicates();
        SetupRepoPersist();

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenSubjectBelongsToOrganization()
    {
        var command = BuildCommand();
        SetupNoDuplicates();
        Subject? captured = null;

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Subject>(), It.IsAny<CancellationToken>()))
            .Callback<Subject, CancellationToken>((s, _) => captured = s)
            .Returns(Task.CompletedTask);

        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        captured.ShouldNotBeNull();
        captured.OrganizationId.ShouldBe(_organizationId);
        captured.Code.Value.ShouldBe("MATH");
    }

    [Test]
    public async Task GivenDuplicateCode_WhenCreating_ThenThrowsInvalidOperationException()
    {
        var command = BuildCommand();

        _repoMock
            .Setup(r =>
                r.ExistsWithCodeAsync(
                    _organizationId,
                    It.IsAny<SubjectCode>(),
                    null,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        _repoMock
            .Setup(r => r.AnyAsync(It.IsAny<ISpecification<Subject>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenDuplicateName_WhenCreating_ThenThrowsInvalidOperationException()
    {
        var command = BuildCommand();

        _repoMock
            .Setup(r =>
                r.ExistsWithCodeAsync(
                    _organizationId,
                    It.IsAny<SubjectCode>(),
                    null,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        _repoMock
            .Setup(r => r.AnyAsync(It.IsAny<ISpecification<Subject>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    private void SetupNoDuplicates()
    {
        _repoMock
            .Setup(r =>
                r.ExistsWithCodeAsync(
                    _organizationId,
                    It.IsAny<SubjectCode>(),
                    null,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        _repoMock
            .Setup(r => r.AnyAsync(It.IsAny<ISpecification<Subject>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }

    private void SetupRepoPersist()
    {
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Subject>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private static CreateSubjectCommand BuildCommand() =>
        new("Математика", "MATH", "#6366F1", null, 0);
}
