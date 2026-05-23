using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods;

/// <summary>
/// Поставщик статистики справочника «Способы оплаты».
/// Реализует <see cref="IDirectoryStatsProvider"/> для отображения в каталоге настроек.
/// </summary>
internal sealed class PaymentMethodStatsProvider(IPaymentMethodRepository repository)
    : IDirectoryStatsProvider
{
    /// <inheritdoc/>
    public DirectoryDescriptor Descriptor =>
        DirectoryCatalog.FindByCode(DirectoryCatalog.PaymentMethods)!;

    /// <inheritdoc/>
    public async Task<DirectoryStats> GetStatsAsync(Guid orgId, CancellationToken ct)
    {
        var activeCount = await repository.CountAsync(
            new PaymentMethodCountSpecification(orgId, isArchived: false),
            ct
        );

        var archivedCount = await repository.CountAsync(
            new PaymentMethodCountSpecification(orgId, isArchived: true),
            ct
        );

        var lastModifiedAt = await repository.GetLastModifiedAtAsync(orgId, ct);

        return new DirectoryStats(
            activeCount,
            archivedCount,
            lastModifiedAt.HasValue
                ? new DateTimeOffset(lastModifiedAt.Value, TimeSpan.Zero)
                : null,
            IsAvailable: true
        );
    }
}
