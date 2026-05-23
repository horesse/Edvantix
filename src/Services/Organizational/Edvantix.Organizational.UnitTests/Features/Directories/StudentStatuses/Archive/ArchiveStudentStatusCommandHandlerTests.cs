using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Features.Directories.StudentStatuses.Archive;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentStatuses.Archive;

public sealed class ArchiveStudentStatusCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IStudentStatusRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly ArchiveStudentStatusCommandHandler _handler;

    public ArchiveStudentStatusCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _handler = new(_tenantMock.Object, _claimsMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingNonSystemStatus_WhenArchiving_ThenShouldArchiveAndSave()
    {
        var status = CreateStatus();
        SetupGetById(status);

        await _handler.Handle(new ArchiveStudentStatusCommand(status.Id), CancellationToken.None);

        status.IsArchived.ShouldBeTrue();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenNonExistentStatus_WhenArchiving_ThenShouldThrowNotFound()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentStatus?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new ArchiveStudentStatusCommand(Guid.CreateVersion7()),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenStatusOfAnotherOrg_WhenArchiving_ThenShouldThrowNotFound()
    {
        var status = new StudentStatus(
            Guid.CreateVersion7(),
            "Другой",
            "OTHER",
            StudentStatusTone.Neutral
        );
        SetupGetById(status);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new ArchiveStudentStatusCommand(status.Id), CancellationToken.None)
                .AsTask()
        );
    }

    [Test]
    public async Task GivenSystemStatus_WhenArchiving_ThenShouldThrowInvalidOperation()
    {
        var status = new StudentStatus(
            _orgId,
            "Активный",
            "ACTIVE",
            StudentStatusTone.Active,
            isSystem: true
        );
        SetupGetById(status);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _handler
                .Handle(new ArchiveStudentStatusCommand(status.Id), CancellationToken.None)
                .AsTask()
        );
    }

    [Test]
    public async Task GivenAlreadyArchivedStatus_WhenArchivingAgain_ThenShouldBeNoop()
    {
        var status = CreateStatus();
        var userId = Guid.CreateVersion7();
        status.Archive(userId);
        SetupGetById(status);

        await _handler.Handle(new ArchiveStudentStatusCommand(status.Id), CancellationToken.None);

        // Архивирование идемпотентно — статус остаётся архивным
        status.IsArchived.ShouldBeTrue();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private StudentStatus CreateStatus() =>
        new(_orgId, "Пользовательский", "CUSTOM", StudentStatusTone.Neutral);

    private void SetupGetById(StudentStatus status) =>
        _repoMock
            .Setup(r => r.GetByIdAsync(status.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);
}
