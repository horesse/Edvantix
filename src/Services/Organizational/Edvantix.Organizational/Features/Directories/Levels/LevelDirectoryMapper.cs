using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Organizational.Features.Directories.Levels;

internal static class LevelDirectoryMapper
{
    internal static LevelDirectoryDto ToDto(
        Level level,
        IReadOnlyList<DirectoryUsageDto>? usage = null
    ) =>
        new(
            level.Id,
            level.Name,
            level.SortOrder,
            level.Description,
            IsArchived: !level.IsActive,
            level.Code.Value,
            level.Tone,
            usage ?? []
        );

    internal static LevelDirectoryListItemDto ToListItemDto(
        Level level,
        IReadOnlyList<DirectoryUsageDto>? usage = null
    ) =>
        new(
            level.Id,
            level.Name,
            level.SortOrder,
            level.Description,
            IsArchived: !level.IsActive,
            level.Code.Value,
            level.Tone,
            usage ?? []
        );
}
