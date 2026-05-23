using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.StudentTags;

/// <summary>
/// Поставщик статистики справочника «Теги студентов».
/// Реализует <see cref="IDirectoryStatsProvider"/> для отображения в каталоге настроек.
/// </summary>
internal sealed class StudentTagStatsProvider(IStudentTagRepository repository)
    : IDirectoryStatsProvider
{
    /// <inheritdoc/>
    public DirectoryDescriptor Descriptor => DirectoryCatalog.FindByCode(DirectoryCatalog.Tags)!;

    /// <inheritdoc/>
    public async Task<DirectoryStats> GetStatsAsync(Guid orgId, CancellationToken ct)
    {
        var activeCount = await repository.CountAsync(
            new StudentTagCountSpecification(orgId, isArchived: false),
            ct
        );

        var archivedCount = await repository.CountAsync(
            new StudentTagCountSpecification(orgId, isArchived: true),
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
