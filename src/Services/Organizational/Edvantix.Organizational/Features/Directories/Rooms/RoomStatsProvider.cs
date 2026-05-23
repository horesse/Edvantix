using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.Rooms;

/// <summary>
/// Поставщик статистики справочника «Кабинеты».
/// Реализует <see cref="IDirectoryStatsProvider"/> для отображения в каталоге настроек.
/// </summary>
internal sealed class RoomStatsProvider(IRoomRepository repository) : IDirectoryStatsProvider
{
    /// <inheritdoc/>
    public DirectoryDescriptor Descriptor => DirectoryCatalog.FindByCode(DirectoryCatalog.Rooms)!;

    /// <inheritdoc/>
    public async Task<DirectoryStats> GetStatsAsync(Guid orgId, CancellationToken ct)
    {
        var activeCount = await repository.CountAsync(
            new RoomCountSpecification(orgId, isArchived: false),
            ct
        );

        var archivedCount = await repository.CountAsync(
            new RoomCountSpecification(orgId, isArchived: true),
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
