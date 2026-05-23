using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Rooms.StatsProvider;

public sealed class RoomStatsProviderTests
{
    private readonly Mock<IRoomRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly RoomStatsProvider _provider;

    public RoomStatsProviderTests()
    {
        _provider = new RoomStatsProvider(_repoMock.Object);
    }

    [Test]
    public void GivenProvider_WhenCheckingDescriptor_ThenShouldMatchCatalog()
    {
        var descriptor = _provider.Descriptor;

        descriptor.ShouldNotBeNull();
        descriptor.Code.ShouldBe(DirectoryCatalog.Rooms);
        descriptor.Badge.ShouldBeNull();
    }

    [Test]
    public async Task GivenOrganizationWithRooms_WhenGettingStats_ThenShouldReturnCorrectCounts()
    {
        // CountAsync вызывается дважды: для активных (isArchived=false), затем для архивных
        _repoMock
            .SetupSequence(r =>
                r.CountAsync(It.IsAny<ISpecification<Room>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(5)
            .ReturnsAsync(2);

        _repoMock
            .Setup(r => r.GetLastModifiedAtAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateTime(2026, 5, 23, 10, 0, 0, DateTimeKind.Utc));

        var stats = await _provider.GetStatsAsync(_orgId, CancellationToken.None);

        stats.ActiveCount.ShouldBe(5);
        stats.ArchivedCount.ShouldBe(2);
        stats.IsAvailable.ShouldBeTrue();
        stats.LastModifiedAt.ShouldNotBeNull();
    }

    [Test]
    public async Task GivenOrganizationWithNoRooms_WhenGettingStats_ThenShouldReturnZeroCounts()
    {
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<Room>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(0);

        _repoMock
            .Setup(r => r.GetLastModifiedAtAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateTime?)null);

        var stats = await _provider.GetStatsAsync(_orgId, CancellationToken.None);

        stats.ActiveCount.ShouldBe(0);
        stats.ArchivedCount.ShouldBe(0);
        stats.LastModifiedAt.ShouldBeNull();
        stats.IsAvailable.ShouldBeTrue();
    }
}
