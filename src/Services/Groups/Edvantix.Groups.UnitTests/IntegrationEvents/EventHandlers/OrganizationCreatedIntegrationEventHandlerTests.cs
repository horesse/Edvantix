namespace Edvantix.Groups.UnitTests.IntegrationEvents.EventHandlers;

public sealed class OrganizationCreatedIntegrationEventHandlerTests
{
    private readonly Mock<ILevelRepository> _levelRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly OrganizationCreatedIntegrationEventHandler _handler;

    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid OwnerProfileId = Guid.CreateVersion7();

    public OrganizationCreatedIntegrationEventHandlerTests()
    {
        _levelRepoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _handler = new(_levelRepoMock.Object);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldAdd8Levels()
    {
        var @event = BuildEvent();

        await _handler.Handle(@event, CancellationToken.None);

        _levelRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()),
            Times.Exactly(8)
        );
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenAllLevelsBelongToOrganization()
    {
        var capturedLevels = new List<Level>();
        _levelRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()))
            .Callback<Level, CancellationToken>((l, _) => capturedLevels.Add(l));

        var @event = BuildEvent();

        await _handler.Handle(@event, CancellationToken.None);

        capturedLevels.ShouldAllBe(l => l.OrganizationId == OrgId);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldSeedExpectedCodes()
    {
        var capturedLevels = new List<Level>();
        _levelRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()))
            .Callback<Level, CancellationToken>((l, _) => capturedLevels.Add(l));

        var @event = BuildEvent();

        await _handler.Handle(@event, CancellationToken.None);

        string[] expectedCodes = ["A1", "A2", "B1", "B2", "C1", "JR", "TN", "PR"];
        capturedLevels.Select(l => l.Code.Value).ShouldBe(expectedCodes, ignoreOrder: true);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenAllLevelsAreActive()
    {
        var capturedLevels = new List<Level>();
        _levelRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()))
            .Callback<Level, CancellationToken>((l, _) => capturedLevels.Add(l));

        var @event = BuildEvent();

        await _handler.Handle(@event, CancellationToken.None);

        capturedLevels.ShouldAllBe(l => l.IsActive);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenSortOrdersAreUnique()
    {
        var capturedLevels = new List<Level>();
        _levelRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()))
            .Callback<Level, CancellationToken>((l, _) => capturedLevels.Add(l));

        var @event = BuildEvent();

        await _handler.Handle(@event, CancellationToken.None);

        capturedLevels.Select(l => l.SortOrder).Distinct().Count().ShouldBe(8);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenShouldSaveOnce()
    {
        var @event = BuildEvent();

        await _handler.Handle(@event, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidEvent_WhenHandling_ThenTonesMatchExpectedMapping()
    {
        var capturedLevels = new List<Level>();
        _levelRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Level>(), It.IsAny<CancellationToken>()))
            .Callback<Level, CancellationToken>((l, _) => capturedLevels.Add(l));

        var @event = BuildEvent();

        await _handler.Handle(@event, CancellationToken.None);

        var byCode = capturedLevels.ToDictionary(l => l.Code.Value);
        byCode["A1"].Tone.ShouldBe(LevelTone.Teal);
        byCode["A2"].Tone.ShouldBe(LevelTone.Teal);
        byCode["B1"].Tone.ShouldBe(LevelTone.Blue);
        byCode["B2"].Tone.ShouldBe(LevelTone.Blue);
        byCode["C1"].Tone.ShouldBe(LevelTone.Indigo);
        byCode["JR"].Tone.ShouldBe(LevelTone.Amber);
        byCode["TN"].Tone.ShouldBe(LevelTone.Amber);
        byCode["PR"].Tone.ShouldBe(LevelTone.Violet);
    }

    private static OrganizationCreatedIntegrationEvent BuildEvent() =>
        new() { OrganizationId = OrgId, OwnerProfileId = OwnerProfileId };
}
