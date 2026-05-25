using Edvantix.Organizational.Domain.LessonTypeAggregate;

namespace Edvantix.Organizational.Features.Directories.LessonTypes;

/// <summary>Расширения для маппинга агрегата <see cref="LessonType"/> в DTO.</summary>
internal static class LessonTypeMappingExtensions
{
    /// <summary>Маппит агрегат в полное DTO (для GET /{id}).</summary>
    public static LessonTypeDto ToDto(this LessonType lt) =>
        new(
            lt.Id,
            lt.Name,
            lt.Code,
            lt.DefaultDurationMinutes,
            lt.Color,
            lt.Icon,
            lt.Order,
            lt.IsArchived,
            lt.CreatedAt,
            lt.LastModifiedAt
        );

    /// <summary>Маппит агрегат в краткое DTO для списка.</summary>
    public static LessonTypeListItemDto ToListItemDto(this LessonType lt) =>
        new(
            lt.Id,
            lt.Name,
            lt.Code,
            lt.DefaultDurationMinutes,
            lt.Color,
            lt.Icon,
            lt.Order,
            lt.IsArchived
        );
}
