using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Features.Directories.StudentStatuses;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentStatuses.StatsProvider;

public sealed class StudentStatusStatsProviderTests
{
    private readonly Mock<IStudentStatusRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly StudentStatusStatsProvider _provider;

    public StudentStatusStatsProviderTests()
    {
        _provider = new StudentStatusStatsProvider(_repoMock.Object);
    }

    [Test]
    public void GivenProvider_WhenCheckingDescriptor_ThenShouldMatchCatalog()
    {
        var descriptor = _provider.Descriptor;

        descriptor.ShouldNotBeNull();
        descriptor.Code.ShouldBe(DirectoryCatalog.StudentStatuses);
        descriptor.Badge.ShouldBe("системный");
    }

    [Test]
    public async Task GivenOrganizationWithStatuses_WhenGettingStats_ThenShouldReturnCorrectCounts()
    {
        _repoMock
            .Setup(r => r.CountActiveAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(4);
        _repoMock
            .Setup(r => r.CountArchivedAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _repoMock
            .Setup(r => r.GetLastModifiedAtAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateTime(2026, 5, 22, 12, 0, 0, DateTimeKind.Utc));

        var stats = await _provider.GetStatsAsync(_orgId, CancellationToken.None);

        stats.ActiveCount.ShouldBe(4);
        stats.ArchivedCount.ShouldBe(1);
        stats.IsAvailable.ShouldBeTrue();
        stats.LastModifiedAt.ShouldNotBeNull();
    }

    [Test]
    public async Task GivenOrganizationWithNoStatuses_WhenGettingStats_ThenShouldReturnZeroCounts()
    {
        _repoMock
            .Setup(r => r.CountActiveAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _repoMock
            .Setup(r => r.CountArchivedAsync(_orgId, It.IsAny<CancellationToken>()))
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
