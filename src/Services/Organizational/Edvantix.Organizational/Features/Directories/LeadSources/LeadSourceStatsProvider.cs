using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.LeadSources;

/// <summary>
/// Поставщик статистики справочника «Источники привлечения».
/// Реализует <see cref="IDirectoryStatsProvider"/> для отображения в каталоге настроек.
/// </summary>
internal sealed class LeadSourceStatsProvider(ILeadSourceRepository repository)
    : IDirectoryStatsProvider
{
    /// <inheritdoc/>
    public DirectoryDescriptor Descriptor => DirectoryCatalog.FindByCode(DirectoryCatalog.Sources)!;

    /// <inheritdoc/>
    public async Task<DirectoryStats> GetStatsAsync(Guid orgId, CancellationToken ct)
    {
        var activeCount = await repository.CountAsync(
            new LeadSourceCountSpecification(orgId, isArchive: false),
            ct
        );

        var archivedCount = await repository.CountAsync(
            new LeadSourceCountSpecification(orgId, isArchive: true),
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
