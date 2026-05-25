using Edvantix.Organizational.Domain.LessonTypeAggregate;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.LessonTypes;

/// <summary>
/// Поставщик статистики справочника «Типы занятий».
/// Реализует <see cref="IDirectoryStatsProvider"/> для отображения в каталоге настроек.
/// </summary>
internal sealed class LessonTypeStatsProvider(ILessonTypeRepository repository)
    : IDirectoryStatsProvider
{
    /// <inheritdoc/>
    public DirectoryDescriptor Descriptor =>
        DirectoryCatalog.FindByCode(DirectoryCatalog.LessonTypes)!;

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
            lastModifiedAt.HasValue
                ? new DateTimeOffset(lastModifiedAt.Value, TimeSpan.Zero)
                : null,
            IsAvailable: true
        );
    }
}
