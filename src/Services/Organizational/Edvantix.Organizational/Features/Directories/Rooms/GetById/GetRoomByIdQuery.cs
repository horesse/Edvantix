using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.Rooms.GetById;

/// <summary>Запрос получения кабинета по идентификатору.</summary>
/// <param name="Id">Идентификатор записи.</param>
[RequirePermission(OrganizationPermissions.View)]
public sealed record GetRoomByIdQuery(Guid Id) : IQuery<RoomDto>;

internal sealed class GetRoomByIdQueryHandler(
    ITenantContext tenantContext,
    IRoomRepository repository,
    IMapper<Room, RoomDto> mapper
) : IQueryHandler<GetRoomByIdQuery, RoomDto>
{
    public async ValueTask<RoomDto> Handle(
        GetRoomByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var room = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (room is null || room.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Room>(query.Id);

        return mapper.Map(room);
    }
}
