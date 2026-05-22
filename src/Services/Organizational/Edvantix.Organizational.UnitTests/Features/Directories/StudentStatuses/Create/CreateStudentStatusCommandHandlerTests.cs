using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Features.Directories.StudentStatuses;
using Edvantix.Organizational.Features.Directories.StudentStatuses.Create;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentStatuses.Create;

public sealed class CreateStudentStatusCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IStudentStatusRepository> _repoMock = new();
    private readonly Mock<IMapper<StudentStatus, StudentStatusDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly CreateStudentStatusCommandHandler _handler;

    public CreateStudentStatusCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<StudentStatus>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _handler = new(_tenantMock.Object, _claimsMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldSaveAndReturnDto()
    {
        var expectedDto = CreateDto();
        var command = new CreateStudentStatusCommand("Активный", "ACTIVE", StudentStatusTone.Active, 0);

        _mapperMock.Setup(m => m.Map(It.IsAny<StudentStatus>())).Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBe(expectedDto);
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenStatusShouldBelongToCurrentOrganization()
    {
        StudentStatus? captured = null;
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<StudentStatus>(), It.IsAny<CancellationToken>()))
            .Callback<StudentStatus, CancellationToken>((s, _) => captured = s)
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map(It.IsAny<StudentStatus>())).Returns(CreateDto());

        var command = new CreateStudentStatusCommand("Активный", "ACTIVE", StudentStatusTone.Active, 0);

        await _handler.Handle(command, CancellationToken.None);

        captured.ShouldNotBeNull();
        captured!.OrganizationId.ShouldBe(_orgId);
        captured.Name.ShouldBe("Активный");
        captured.Code.ShouldBe("ACTIVE");
        captured.IsSystem.ShouldBeFalse();
    }

    private static StudentStatusDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "Активный",
            "ACTIVE",
            StudentStatusTone.Active,
            false,
            false,
            0,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
