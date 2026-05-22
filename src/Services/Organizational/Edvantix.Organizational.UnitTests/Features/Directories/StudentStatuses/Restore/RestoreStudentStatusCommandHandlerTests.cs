using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Features.Directories.StudentStatuses.Restore;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentStatuses.Restore;

public sealed class RestoreStudentStatusCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IStudentStatusRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly RestoreStudentStatusCommandHandler _handler;

    public RestoreStudentStatusCommandHandlerTests()
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
    public async Task GivenArchivedNonSystemStatus_WhenRestoring_ThenShouldRestoreAndSave()
    {
        var status = CreateArchivedStatus();
        SetupGetById(status);

        await _handler.Handle(new RestoreStudentStatusCommand(status.Id), CancellationToken.None);

        status.IsArchived.ShouldBeFalse();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenNonExistentStatus_WhenRestoring_ThenShouldThrowNotFound()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentStatus?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new RestoreStudentStatusCommand(Guid.CreateVersion7()),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenSystemStatus_WhenRestoring_ThenShouldThrowInvalidOperation()
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
                .Handle(new RestoreStudentStatusCommand(status.Id), CancellationToken.None)
                .AsTask()
        );
    }

    [Test]
    public async Task GivenActiveStatus_WhenRestoringAgain_ThenShouldBeNoop()
    {
        var status = new StudentStatus(
            _orgId,
            "Пользовательский",
            "CUSTOM",
            StudentStatusTone.Neutral
        );
        SetupGetById(status);

        await _handler.Handle(new RestoreStudentStatusCommand(status.Id), CancellationToken.None);

        status.IsArchived.ShouldBeFalse();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private StudentStatus CreateArchivedStatus()
    {
        var status = new StudentStatus(
            _orgId,
            "Пользовательский",
            "CUSTOM",
            StudentStatusTone.Neutral
        );
        status.Archive(Guid.CreateVersion7());
        return status;
    }

    private void SetupGetById(StudentStatus status) =>
        _repoMock
            .Setup(r => r.GetByIdAsync(status.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);
}
