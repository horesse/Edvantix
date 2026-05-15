namespace Edvantix.Organizational.UnitTests.Features.Levels.List;

public sealed class GetLevelsQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Mock<IMapper<Level, LevelDto>> _mapperMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetLevelsQueryHandler _handler;

    public GetLevelsQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenActiveLevels_WhenHandling_ThenReturnsAllMappedLevels()
    {
        var level1 = CreateLevel(sortOrder: 1);
        var level2 = CreateLevel(sortOrder: 2);
        var dto1 = CreateDto(level1.Id, sortOrder: 1);
        var dto2 = CreateDto(level2.Id, sortOrder: 2);

        SetupRepository([level1, level2], includeInactive: false);
        _mapperMock.Setup(m => m.Map(level1)).Returns(dto1);
        _mapperMock.Setup(m => m.Map(level2)).Returns(dto2);

        var result = await _handler.Handle(new GetLevelsQuery(), CancellationToken.None);

        result.Count.ShouldBe(2);
        result[0].Id.ShouldBe(level1.Id);
        result[1].Id.ShouldBe(level2.Id);
    }

    [Test]
    public async Task GivenNoLevels_WhenHandling_ThenReturnsEmptyList()
    {
        SetupRepository([], includeInactive: false);

        var result = await _handler.Handle(new GetLevelsQuery(), CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenIncludeInactiveFalse_WhenHandling_ThenCallsRepositoryWithFalse()
    {
        SetupRepository([], includeInactive: false);

        await _handler.Handle(new GetLevelsQuery(IncludeInactive: false), CancellationToken.None);

        _repoMock.Verify(
            r => r.ListByOrganizationAsync(_organizationId, false, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenIncludeInactiveTrue_WhenHandling_ThenCallsRepositoryWithTrue()
    {
        SetupRepository([], includeInactive: true);

        await _handler.Handle(new GetLevelsQuery(IncludeInactive: true), CancellationToken.None);

        _repoMock.Verify(
            r => r.ListByOrganizationAsync(_organizationId, true, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenLevels_WhenHandling_ThenMapperCalledForEachLevel()
    {
        var level1 = CreateLevel(sortOrder: 1);
        var level2 = CreateLevel(sortOrder: 2);

        SetupRepository([level1, level2], includeInactive: false);
        _mapperMock.Setup(m => m.Map(It.IsAny<Level>())).Returns(CreateDto(Guid.CreateVersion7()));

        await _handler.Handle(new GetLevelsQuery(), CancellationToken.None);

        _mapperMock.Verify(m => m.Map(level1), Times.Once);
        _mapperMock.Verify(m => m.Map(level2), Times.Once);
    }

    private void SetupRepository(IReadOnlyCollection<Level> levels, bool includeInactive) =>
        _repoMock
            .Setup(r =>
                r.ListByOrganizationAsync(
                    _organizationId,
                    includeInactive,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(levels);

    private Level CreateLevel(short sortOrder = 1) =>
        new(_organizationId, LevelCode.From("A1"), "Beginner", null, LevelTone.Blue, sortOrder);

    private static LevelDto CreateDto(Guid id, short sortOrder = 1) =>
        new(id, "A1", "Beginner", null, LevelTone.Blue, sortOrder, IsActive: true, UsageCount: 0);
}
