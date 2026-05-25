using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.Levels;

/// <summary>
/// Локальный поставщик статистики справочника «Уровни».
/// Реализует <see cref="IDirectoryStatsProvider"/> через прямой запрос к <see cref="ILevelRepository"/>.
/// </summary>
internal sealed class LevelDirectoryStatsProvider(ILevelRepository repository)
    : IDirectoryStatsProvider
{
    /// <inheritdoc/>
    public DirectoryDescriptor Descriptor =>
        DirectoryCatalog.FindByCode(DirectoryCatalog.Levels)!;

    /// <inheritdoc/>
    public async Task<DirectoryStats> GetStatsAsync(Guid orgId, CancellationToken ct)
    {
        var (active, archived) = await repository.GetStatsAsync(orgId, ct);

        return new DirectoryStats(
            active,
            archived,
            LastModifiedAt: null,
            IsAvailable: true
        );
    }
}
