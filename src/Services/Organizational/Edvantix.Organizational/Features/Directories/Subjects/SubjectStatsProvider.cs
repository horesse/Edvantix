using Edvantix.Organizational.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.Subjects;

/// <summary>
/// Поставщик статистики справочника «Предметы».
/// Реализует <see cref="IDirectoryStatsProvider"/> для отображения в каталоге настроек.
/// </summary>
internal sealed class SubjectStatsProvider(ISubjectRepository repository) : IDirectoryStatsProvider
{
    /// <inheritdoc/>
    public DirectoryDescriptor Descriptor => DirectoryCatalog.FindByCode(DirectoryCatalog.Subjects)!;

    /// <inheritdoc/>
    public async Task<DirectoryStats> GetStatsAsync(Guid orgId, CancellationToken ct)
    {
        var (activeCount, archivedCount, lastModifiedAt) = await repository.GetStatsAsync(
            orgId,
            ct
        );

        return new DirectoryStats(
            activeCount,
            archivedCount,
            lastModifiedAt.HasValue ? new DateTimeOffset(lastModifiedAt.Value, TimeSpan.Zero) : null,
            IsAvailable: true
        );
    }
}
