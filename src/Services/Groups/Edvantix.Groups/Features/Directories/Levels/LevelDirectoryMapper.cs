using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Groups.Features.Directories.Levels;

internal static class LevelDirectoryMapper
{
    internal static LevelDirectoryDto ToDto(Level level) =>
        new(
            level.Id,
            level.Name,
            level.SortOrder,
            level.Description,
            IsArchived: !level.IsActive,
            level.Code.Value,
            level.Tone
        );

    internal static LevelDirectoryListItemDto ToListItemDto(Level level) =>
        new(
            level.Id,
            level.Name,
            level.SortOrder,
            level.Description,
            IsArchived: !level.IsActive
        );
}
