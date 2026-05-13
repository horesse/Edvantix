using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.Features.Rooms;

public sealed class RoomDtoMapper : Mapper<Room, RoomDto>
{
    public override RoomDto Map(Room source) =>
        new(source.Id, source.OrganizationId, source.Label, source.Floor, source.Seats);
}
