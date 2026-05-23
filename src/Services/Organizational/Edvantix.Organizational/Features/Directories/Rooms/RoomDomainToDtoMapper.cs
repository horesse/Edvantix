using Edvantix.Chassis.Mapper;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.Features.Directories.Rooms;

/// <summary>Маппер <see cref="Room"/> → <see cref="RoomDto"/>.</summary>
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
            source.IsArchived,
            source.Order,
            source.OrganizationId,
            source.CreatedAt,
            source.LastModifiedAt,
            source.CreatedBy,
            source.LastModifiedBy
        );
}

/// <summary>Маппер <see cref="Room"/> → <see cref="RoomListItemDto"/>.</summary>
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
            source.IsArchived,
            source.Order
        );
}
