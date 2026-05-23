using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Features.Directories.StudentTags;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.StatsProvider;

public sealed class StudentTagStatsProviderTests
{
    private readonly Mock<IStudentTagRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly StudentTagStatsProvider _provider;

    public StudentTagStatsProviderTests()
    {
        _provider = new StudentTagStatsProvider(_repoMock.Object);
    }

    [Test]
    public void GivenProvider_WhenCheckingDescriptor_ThenShouldMatchCatalog()
    {
        var descriptor = _provider.Descriptor;

        descriptor.ShouldNotBeNull();
        descriptor.Code.ShouldBe(DirectoryCatalog.Tags);
        descriptor.Badge.ShouldBeNull();
    }

    [Test]
    public async Task GivenOrganizationWithTags_WhenGettingStats_ThenShouldReturnCorrectCounts()
    {
        // CountAsync вызывается дважды: для активных (isArchived=false), затем для архивных
        _repoMock
            .SetupSequence(r =>
                r.CountAsync(It.IsAny<ISpecification<StudentTag>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(7)
            .ReturnsAsync(3);

        _repoMock
            .Setup(r => r.GetLastModifiedAtAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateTime(2026, 5, 24, 10, 0, 0, DateTimeKind.Utc));

        var stats = await _provider.GetStatsAsync(_orgId, CancellationToken.None);

        stats.ActiveCount.ShouldBe(7);
        stats.ArchivedCount.ShouldBe(3);
        stats.IsAvailable.ShouldBeTrue();
        stats.LastModifiedAt.ShouldNotBeNull();
    }

    [Test]
    public async Task GivenOrganizationWithNoTags_WhenGettingStats_ThenShouldReturnZeroCounts()
    {
        _repoMock
            .Setup(r =>
                r.CountAsync(It.IsAny<ISpecification<StudentTag>>(), It.IsAny<CancellationToken>())
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
