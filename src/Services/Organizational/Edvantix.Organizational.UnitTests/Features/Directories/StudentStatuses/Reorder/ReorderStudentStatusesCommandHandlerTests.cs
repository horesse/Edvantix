using Edvantix.Organizational.Features.Directories.StudentStatuses.Reorder;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentStatuses.Reorder;

public sealed class ReorderStudentStatusesCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IStudentStatusRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly ReorderStudentStatusesCommandHandler _handler;

    public ReorderStudentStatusesCommandHandlerTests()
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
    public async Task GivenThreeStatuses_WhenReordering_ThenOrderShouldMatchIndexPosition()
    {
        var s1 = CreateStatus("A");
        var s2 = CreateStatus("B");
        var s3 = CreateStatus("C");
        SetupList([s1, s2, s3]);

        await _handler.Handle(
            new ReorderStudentStatusesCommand([s3.Id, s1.Id, s2.Id]),
            CancellationToken.None
        );

        s3.Order.ShouldBe(0);
        s1.Order.ShouldBe(1);
        s2.Order.ShouldBe(2);
    }

    [Test]
    public async Task GivenReordering_WhenCalled_ThenShouldSaveOnce()
    {
        var s = CreateStatus("A");
        SetupList([s]);

        await _handler.Handle(new ReorderStudentStatusesCommand([s.Id]), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenUnknownId_WhenReordering_ThenShouldBeIgnored()
    {
        var s = CreateStatus("A");
        SetupList([s]);
        var unknownId = Guid.CreateVersion7();

        await _handler.Handle(
            new ReorderStudentStatusesCommand([unknownId, s.Id]),
            CancellationToken.None
        );

        s.Order.ShouldBe(1);
    }

    private StudentStatus CreateStatus(string code)
    {
        var status = new StudentStatus(_orgId, $"Статус {code}", code, StudentStatusTone.Neutral);
        status.Id = Guid.CreateVersion7();
        return status;
    }

    private void SetupList(IReadOnlyList<StudentStatus> items) =>
        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<StudentStatus>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(items);
}
