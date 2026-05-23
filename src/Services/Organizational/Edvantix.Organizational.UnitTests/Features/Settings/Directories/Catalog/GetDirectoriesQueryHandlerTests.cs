using Edvantix.Organizational.Features.Settings.Directories;
using Edvantix.Organizational.Features.Settings.Directories.Catalog;

namespace Edvantix.Organizational.UnitTests.Features.Settings.Directories.Catalog;

public sealed class GetDirectoriesQueryHandlerTests : IDisposable
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILogger<GetDirectoriesQueryHandler>> _loggerMock = new();
    private readonly IFusionCache _cache = new FusionCache(new FusionCacheOptions());
    private readonly Guid _orgId = Guid.CreateVersion7();

    public GetDirectoriesQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
    }

    [Test]
    public async Task GivenNoProvidersRegistered_WhenQuerying_ThenShouldReturnExactly8Items()
    {
        var handler = BuildHandler([]);

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        result.Count.ShouldBe(8);
    }

    [Test]
    public async Task GivenNoProvidersRegistered_WhenQuerying_ThenOrderShouldMatchDirectoryCatalog()
    {
        var handler = BuildHandler([]);

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        var expectedCodes = DirectoryCatalog.All.Select(d => d.Code).ToArray();
        result.Select(d => d.Code).ShouldBe(expectedCodes);
    }

    [Test]
    public async Task GivenNoProvidersRegistered_WhenQuerying_ThenAllDirectoriesShouldHaveIsAvailableFalse()
    {
        var handler = BuildHandler([]);

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        result.ShouldAllBe(d => !d.IsAvailable);
    }

    [Test]
    public async Task GivenProviderReturnsStats_WhenQuerying_ThenStatsMergedWithCatalogMetadata()
    {
        var descriptor = DirectoryCatalog.All[0]; // levels
        var expectedStats = new DirectoryStats(10, 3, DateTimeOffset.UtcNow, true);
        var providerMock = CreateProviderMock(descriptor, expectedStats);
        var handler = BuildHandler([providerMock.Object]);

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        var levelsDto = result.Single(d => d.Code == DirectoryCatalog.Levels);
        levelsDto.Name.ShouldBe(descriptor.Name);
        levelsDto.Description.ShouldBe(descriptor.Description);
        levelsDto.Icon.ShouldBe(descriptor.Icon);
        levelsDto.ActiveCount.ShouldBe(10);
        levelsDto.ArchivedCount.ShouldBe(3);
        levelsDto.IsAvailable.ShouldBeTrue();
    }

    [Test]
    public async Task GivenOneProviderThrows_WhenQuerying_ThenThatDirectoryShouldHaveIsAvailableFalse()
    {
        var descriptor = DirectoryCatalog.All[4]; // rooms
        var providerMock = new Mock<IDirectoryStatsProvider>();
        providerMock.Setup(p => p.Descriptor).Returns(descriptor);
        providerMock
            .Setup(p => p.GetStatsAsync(_orgId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB недоступна"));
        var handler = BuildHandler([providerMock.Object]);

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        var roomsDto = result.Single(d => d.Code == DirectoryCatalog.Rooms);
        roomsDto.IsAvailable.ShouldBeFalse();
        roomsDto.ActiveCount.ShouldBe(0);
    }

    [Test]
    public async Task GivenOneProviderThrows_WhenQuerying_ThenOtherDirectoriesShouldReturnStats()
    {
        var failDescriptor = DirectoryCatalog.All[4]; // rooms
        var okDescriptor = DirectoryCatalog.All[0]; // levels
        var okStats = new DirectoryStats(5, 1, null, true);

        var failProviderMock = new Mock<IDirectoryStatsProvider>();
        failProviderMock.Setup(p => p.Descriptor).Returns(failDescriptor);
        failProviderMock
            .Setup(p => p.GetStatsAsync(_orgId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("ошибка"));

        var okProviderMock = CreateProviderMock(okDescriptor, okStats);

        var handler = BuildHandler([failProviderMock.Object, okProviderMock.Object]);

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        result.Single(d => d.Code == DirectoryCatalog.Rooms).IsAvailable.ShouldBeFalse();
        var levelsDto = result.Single(d => d.Code == DirectoryCatalog.Levels);
        levelsDto.IsAvailable.ShouldBeTrue();
        levelsDto.ActiveCount.ShouldBe(5);
    }

    [Test]
    public async Task GivenOneProviderThrowsOperationCancelled_WhenMainTokenNotCancelled_ThenShouldReturnStubForThatDirectory()
    {
        // Simulates a provider that times out internally (not due to the main request token).
        var descriptor = DirectoryCatalog.All[4]; // rooms
        var providerMock = new Mock<IDirectoryStatsProvider>();
        providerMock.Setup(p => p.Descriptor).Returns(descriptor);
        providerMock
            .Setup(p => p.GetStatsAsync(_orgId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException("provider internal timeout"));
        var handler = BuildHandler([providerMock.Object]);

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        result.Count.ShouldBe(8);
        result.Single(d => d.Code == DirectoryCatalog.Rooms).IsAvailable.ShouldBeFalse();
    }

    [Test]
    public async Task GivenAllProvidersRegistered_WhenQuerying_ThenShouldReturnExactly8Items()
    {
        var providerMocks = DirectoryCatalog
            .All.Select(d => CreateProviderMock(d, new DirectoryStats(1, 0, null, true)))
            .ToArray();
        var handler = BuildHandler(providerMocks.Select(m => m.Object).ToList());

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        result.Count.ShouldBe(8);
    }

    [Test]
    public async Task GivenStudentStatusesDescriptor_WhenQuerying_ThenBadgeShouldBePreserved()
    {
        var handler = BuildHandler([]);

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        var statusesDto = result.Single(d => d.Code == DirectoryCatalog.StudentStatuses);
        statusesDto.Badge.ShouldBe("системный");
    }

    [Test]
    public async Task GivenLevelsDescriptor_WhenQuerying_ThenBadgeShouldBeNull()
    {
        var handler = BuildHandler([]);

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        var levelsDto = result.Single(d => d.Code == DirectoryCatalog.Levels);
        levelsDto.Badge.ShouldBeNull();
    }

    public void Dispose() => _cache.Dispose();

    private GetDirectoriesQueryHandler BuildHandler(
        IEnumerable<IDirectoryStatsProvider> directoryProviders
    ) => new(_tenantMock.Object, directoryProviders, _cache, _loggerMock.Object);

    private Mock<IDirectoryStatsProvider> CreateProviderMock(
        DirectoryDescriptor descriptor,
        DirectoryStats stats
    )
    {
        var mock = new Mock<IDirectoryStatsProvider>();
        mock.Setup(p => p.Descriptor).Returns(descriptor);
        mock.Setup(p => p.GetStatsAsync(_orgId, It.IsAny<CancellationToken>())).ReturnsAsync(stats);
        return mock;
    }
}
