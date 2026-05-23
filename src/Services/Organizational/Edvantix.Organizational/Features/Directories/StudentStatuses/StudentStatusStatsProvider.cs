using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses;

/// <summary>
/// Поставщик статистики справочника «Статусы студентов».
/// Реализует <see cref="IDirectoryStatsProvider"/> для отображения в каталоге настроек.
/// </summary>
internal sealed class StudentStatusStatsProvider(IStudentStatusRepository repository)
    : IDirectoryStatsProvider
{
    /// <inheritdoc/>
    public DirectoryDescriptor Descriptor =>
        DirectoryCatalog.FindByCode(DirectoryCatalog.StudentStatuses)!;

    /// <inheritdoc/>
    public async Task<DirectoryStats> GetStatsAsync(Guid orgId, CancellationToken ct)
    {
        var activeCount = await repository.CountAsync(
            new StudentStatusCountSpecification(orgId, isArchived: false),
            ct
        );

        var archivedCount = await repository.CountAsync(
            new StudentStatusCountSpecification(orgId, isArchived: true),
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
