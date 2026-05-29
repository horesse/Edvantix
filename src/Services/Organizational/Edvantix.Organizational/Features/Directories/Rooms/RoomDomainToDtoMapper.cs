using Edvantix.Chassis.Mapper;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.Features.Directories.Rooms;

/// <summary>Маппер <see cref="Room"/> → <see cref="RoomDto"/>.</summary>
/// <remarks>
/// Поле <see cref="RoomDto.Usage"/> маппер не заполняет — обогащение выполняется в хендлере
/// через выражение <c>with { Usage = ... }</c> после batch-запроса к сервису Groups.
/// </remarks>
public sealed class RoomDtoMapper : Mapper<Room, RoomDto>
{
    /// <inheritdoc/>
    public override RoomDto Map(Room source) =>
        new(
            source.Id,
            source.Name,
            source.Capacity,
            source.Floor,
            source.RoomType,
            source.IsDeleted,
            source.Order,
            source.OrganizationId,
            source.CreatedAt,
            source.LastModifiedAt,
            source.CreatedBy,
            source.LastModifiedBy,
            Usage: []
        );
}

/// <summary>Маппер <see cref="Room"/> → <see cref="RoomListItemDto"/>.</summary>
/// <remarks>
/// Поле <see cref="RoomListItemDto.Usage"/> маппер не заполняет — обогащение выполняется в хендлере
/// через выражение <c>with { Usage = ... }</c> после batch-запроса к сервису Groups.
/// </remarks>
public sealed class RoomListItemDtoMapper : Mapper<Room, RoomListItemDto>
{
    /// <inheritdoc/>
    public override RoomListItemDto Map(Room source) =>
        new(
            source.Id,
            source.Name,
            source.Capacity,
            source.Floor,
            source.RoomType,
            source.IsDeleted,
            source.Order,
            Usage: []
        );
}
