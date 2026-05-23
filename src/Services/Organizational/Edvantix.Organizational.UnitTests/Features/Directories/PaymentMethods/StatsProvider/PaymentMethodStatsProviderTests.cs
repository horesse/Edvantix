using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Features.Directories.PaymentMethods;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.StatsProvider;

public sealed class PaymentMethodStatsProviderTests
{
    private readonly Mock<IPaymentMethodRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly PaymentMethodStatsProvider _provider;

    public PaymentMethodStatsProviderTests()
    {
        _provider = new PaymentMethodStatsProvider(_repoMock.Object);
    }

    [Test]
    public void GivenProvider_WhenCheckingDescriptor_ThenShouldMatchCatalog()
    {
        var descriptor = _provider.Descriptor;

        descriptor.ShouldNotBeNull();
        descriptor.Code.ShouldBe(DirectoryCatalog.PaymentMethods);
        descriptor.Badge.ShouldBeNull();
    }

    [Test]
    public async Task GivenOrganizationWithPaymentMethods_WhenGettingStats_ThenShouldReturnCorrectCounts()
    {
        // CountAsync вызывается дважды: для активных (isArchived=false), затем для архивных
        _repoMock
            .SetupSequence(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<PaymentMethod>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(5)
            .ReturnsAsync(2);

        _repoMock
            .Setup(r => r.GetLastModifiedAtAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateTime(2026, 5, 23, 12, 0, 0, DateTimeKind.Utc));

        var stats = await _provider.GetStatsAsync(_orgId, CancellationToken.None);

        stats.ActiveCount.ShouldBe(5);
        stats.ArchivedCount.ShouldBe(2);
        stats.IsAvailable.ShouldBeTrue();
        stats.LastModifiedAt.ShouldNotBeNull();
    }

    [Test]
    public async Task GivenOrganizationWithNoPaymentMethods_WhenGettingStats_ThenShouldReturnZeroCounts()
    {
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<PaymentMethod>>(),
                    It.IsAny<CancellationToken>()
                )
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
