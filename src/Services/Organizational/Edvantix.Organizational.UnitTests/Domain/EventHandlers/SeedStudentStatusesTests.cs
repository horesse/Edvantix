using Edvantix.Organizational.Domain.EventHandlers.OrganizationEventHandlers;

namespace Edvantix.Organizational.UnitTests.Domain.EventHandlers;

public sealed class SeedStudentStatusesTests
{
    private readonly Mock<IStudentStatusRepository> _studentStatusRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly SeedStudentStatuses _handler;

    private static readonly Guid OrgId = Guid.CreateVersion7();

    public SeedStudentStatusesTests()
    {
        _studentStatusRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _handler = new(_studentStatusRepoMock.Object);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldAdd4StudentStatuses()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, Guid.CreateVersion7());

        await _handler.Handle(@event, CancellationToken.None);

        _studentStatusRepoMock.Verify(
            r =>
                r.AddRangeAsync(
                    It.Is<List<StudentStatus>>(statuses => statuses.Count == 4),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenAllStatusesBelongToOrganization()
    {
        List<StudentStatus>? capturedStatuses = null;
        _studentStatusRepoMock
            .Setup(r =>
                r.AddRangeAsync(It.IsAny<List<StudentStatus>>(), It.IsAny<CancellationToken>())
            )
            .Callback<List<StudentStatus>, CancellationToken>(
                (statuses, _) => capturedStatuses = statuses
            );

        var @event = new OrganizationCreatedDomainEvent(OrgId, Guid.CreateVersion7());

        await _handler.Handle(@event, CancellationToken.None);

        capturedStatuses!.ShouldAllBe(s => s.OrganizationId == OrgId);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldSaveOnce()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, Guid.CreateVersion7());

        await _handler.Handle(@event, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
