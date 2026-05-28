using Edvantix.Organizational.Domain.EventHandlers.OrganizationEventHandlers;

namespace Edvantix.Organizational.UnitTests.Domain.EventHandlers;

public sealed class SeedLevelsTests
{
    private readonly Mock<ILevelRepository> _levelRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly SeedLevels _handler;

    private static readonly Guid OrgId = Guid.CreateVersion7();

    public SeedLevelsTests()
    {
        _levelRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _handler = new(_levelRepoMock.Object);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldAdd5Levels()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, Guid.CreateVersion7());

        await _handler.Handle(@event, CancellationToken.None);

        _levelRepoMock.Verify(
            r =>
                r.AddRange(
                    It.Is<List<Level>>(levels => levels.Count == 5),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenAllLevelsBelongToOrganization()
    {
        List<Level>? capturedLevels = null;
        _levelRepoMock
            .Setup(r => r.AddRange(It.IsAny<List<Level>>(), It.IsAny<CancellationToken>()))
            .Callback<List<Level>, CancellationToken>((levels, _) => capturedLevels = levels);

        var @event = new OrganizationCreatedDomainEvent(OrgId, Guid.CreateVersion7());

        await _handler.Handle(@event, CancellationToken.None);

        capturedLevels!.ShouldAllBe(l => l.OrganizationId == OrgId);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldSaveOnce()
    {
        var @event = new OrganizationCreatedDomainEvent(OrgId, Guid.CreateVersion7());

        await _handler.Handle(@event, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
