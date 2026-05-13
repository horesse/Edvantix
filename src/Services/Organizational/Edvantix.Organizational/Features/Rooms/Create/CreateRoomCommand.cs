using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Rooms.Create;

[Transactional]
[RequirePermission(OrganizationPermissions.Rooms)]
public sealed record CreateRoomCommand(string Label, short Floor, short Seats) : ICommand<Guid>;

internal sealed class CreateRoomCommandHandler(
    ITenantContext tenantContext,
    IRoomRepository repository
) : ICommandHandler<CreateRoomCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateRoomCommand command,
        CancellationToken cancellationToken
    )
    {
        var room = new Room(
            tenantContext.OrganizationId,
            command.Label,
            command.Floor,
            command.Seats
        );

        await repository.AddAsync(room, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return room.Id;
    }
}
