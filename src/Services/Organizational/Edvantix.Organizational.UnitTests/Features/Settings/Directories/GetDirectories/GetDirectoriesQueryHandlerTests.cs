using Edvantix.Organizational.Features.Settings.Directories;
using Edvantix.Organizational.Features.Settings.Directories.GetDirectories;

namespace Edvantix.Organizational.UnitTests.Features.Settings.Directories.GetDirectories;

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
    public async Task GivenAllStubProviders_WhenHandling_ThenShouldReturnExactly8Items()
    {
        var handler = CreateHandler(AllStubs());

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        result.Count.ShouldBe(DirectoryCatalog.All.Count);
    }

    [Test]
    public async Task GivenAllStubProviders_WhenHandling_ThenOrderShouldMatchDirectoryCatalog()
    {
        var handler = CreateHandler(AllStubs());

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        var expectedCodes = DirectoryCatalog.All.Select(d => d.Code).ToList();
        result.Select(r => r.Code).ShouldBe(expectedCodes);
    }

    [Test]
    public async Task GivenAllStubProviders_WhenHandling_ThenAllItemsShouldBeUnavailable()
    {
        var handler = CreateHandler(AllStubs());

        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        result.ShouldAllBe(r => !r.IsAvailable);
    }

    [Test]
    public async Task GivenOneProviderThrows_WhenHandling_ThenThatItemShouldBeUnavailable()
    {
        var failingDescriptor = DirectoryCatalog.All[0];
        var failingMock = CreateProviderMock(failingDescriptor);
        failingMock
            .Setup(p => p.GetStatsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB failure"));

        var providers = new List<IDirectoryStatsProvider> { failingMock.Object };
        providers.AddRange(DirectoryCatalog.All.Skip(1).Select(d => (IDirectoryStatsProvider)new StubDirectoryStatsProvider(d)));

        var handler = CreateHandler(providers);
        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        result.Count.ShouldBe(8);
        result[0].Code.ShouldBe(failingDescriptor.Code);
        result[0].IsAvailable.ShouldBeFalse();
    }

    [Test]
    public async Task GivenOneProviderThrows_WhenHandling_ThenOtherItemsShouldStillReturn()
    {
        var failingMock = CreateProviderMock(DirectoryCatalog.All[0]);
        failingMock
            .Setup(p => p.GetStatsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("timeout"));

        var providers = new List<IDirectoryStatsProvider> { failingMock.Object };
        providers.AddRange(DirectoryCatalog.All.Skip(1).Select(d => (IDirectoryStatsProvider)new StubDirectoryStatsProvider(d)));

        var handler = CreateHandler(providers);
        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        result.Count.ShouldBe(8);
        // Первый — ошибка, остальные — заглушки, все IsAvailable=false, но все возвращены
        result.Skip(1).Select(r => r.Code)
            .ShouldBe(DirectoryCatalog.All.Skip(1).Select(d => d.Code));
    }

    [Test]
    public async Task GivenRealProvider_WhenHandling_ThenStatsShouldMergeWithCatalogMetadata()
    {
        var descriptor = DirectoryCatalog.All[0];
        var lastModified = new DateTimeOffset(2026, 5, 21, 10, 0, 0, TimeSpan.Zero);
        var expectedStats = new DirectoryStats(
            ActiveCount: 5,
            ArchivedCount: 2,
            LastModifiedAt: lastModified,
            IsAvailable: true
        );
        var realMock = CreateProviderMock(descriptor);
        realMock
            .Setup(p => p.GetStatsAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedStats);

        var providers = new List<IDirectoryStatsProvider> { realMock.Object };
        providers.AddRange(DirectoryCatalog.All.Skip(1).Select(d => (IDirectoryStatsProvider)new StubDirectoryStatsProvider(d)));

        var handler = CreateHandler(providers);
        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        var dto = result.Single(r => r.Code == descriptor.Code);
        dto.Name.ShouldBe(descriptor.Name);
        dto.Description.ShouldBe(descriptor.Description);
        dto.Icon.ShouldBe(descriptor.Icon);
        dto.Badge.ShouldBe(descriptor.Badge);
        dto.ActiveCount.ShouldBe(5);
        dto.ArchivedCount.ShouldBe(2);
        dto.LastModifiedAt.ShouldBe(lastModified);
        dto.IsAvailable.ShouldBeTrue();
    }

    [Test]
    public async Task GivenRealProviderRegisteredAfterStub_WhenHandling_ThenRealProviderWins()
    {
        var descriptor = DirectoryCatalog.All[0];
        var stub = new StubDirectoryStatsProvider(descriptor);

        var realStats = new DirectoryStats(10, 1, DateTimeOffset.UtcNow, IsAvailable: true);
        var realMock = CreateProviderMock(descriptor);
        realMock
            .Setup(p => p.GetStatsAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(realStats);

        // stub зарегистрирован первым, real — после (last-wins семантика)
        var providers = new List<IDirectoryStatsProvider>();
        providers.Add(stub);
        providers.AddRange(DirectoryCatalog.All.Skip(1).Select(d => (IDirectoryStatsProvider)new StubDirectoryStatsProvider(d)));
        providers.Add(realMock.Object); // real провайдер после заглушки

        var handler = CreateHandler(providers);
        var result = await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        var dto = result.Single(r => r.Code == descriptor.Code);
        dto.IsAvailable.ShouldBeTrue();
        dto.ActiveCount.ShouldBe(10);
    }

    [Test]
    public async Task GivenProviderThrowsOperationCancelled_WhenHandling_ThenCancellationPropagates()
    {
        var cancellingMock = CreateProviderMock(DirectoryCatalog.All[0]);
        cancellingMock
            .Setup(p => p.GetStatsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var providers = new List<IDirectoryStatsProvider> { cancellingMock.Object };
        providers.AddRange(DirectoryCatalog.All.Skip(1).Select(d => (IDirectoryStatsProvider)new StubDirectoryStatsProvider(d)));

        var handler = CreateHandler(providers);

        await Should.ThrowAsync<OperationCanceledException>(
            () => handler.Handle(new GetDirectoriesQuery(), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenEachProviderCallsOrgId_WhenHandling_ThenOrgIdPassedFromTenantContext()
    {
        var providerMock = CreateProviderMock(DirectoryCatalog.All[0]);
        providerMock
            .Setup(p => p.GetStatsAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DirectoryStats(1, 0, null, true));

        var providers = new List<IDirectoryStatsProvider> { providerMock.Object };
        providers.AddRange(DirectoryCatalog.All.Skip(1).Select(d => (IDirectoryStatsProvider)new StubDirectoryStatsProvider(d)));

        var handler = CreateHandler(providers);
        await handler.Handle(new GetDirectoriesQuery(), CancellationToken.None);

        providerMock.Verify(p => p.GetStatsAsync(_orgId, It.IsAny<CancellationToken>()), Times.Once);
    }

    public void Dispose() => _cache.Dispose();

    private GetDirectoriesQueryHandler CreateHandler(IEnumerable<IDirectoryStatsProvider> providers) =>
        new(_tenantMock.Object, providers, _cache, _loggerMock.Object);

    private static List<IDirectoryStatsProvider> AllStubs() =>
        DirectoryCatalog.All
            .Select(d => (IDirectoryStatsProvider)new StubDirectoryStatsProvider(d))
            .ToList();

    private static Mock<IDirectoryStatsProvider> CreateProviderMock(DirectoryDescriptor descriptor)
    {
        var mock = new Mock<IDirectoryStatsProvider>();
        mock.Setup(p => p.Descriptor).Returns(descriptor);
        return mock;
    }
}
