using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Rooms.Update;

[Transactional]
[RequirePermission(OrganizationPermissions.Rooms)]
public sealed record UpdateRoomCommand(Guid Id, string Label, short Floor, short Seats) : ICommand;

internal sealed class UpdateRoomCommandHandler(
    ITenantContext tenantContext,
    IRoomRepository repository
) : ICommandHandler<UpdateRoomCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateRoomCommand command,
        CancellationToken cancellationToken
    )
    {
        var room = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (room is null || room.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Room>(command.Id);

        room.Update(command.Label, command.Floor, command.Seats);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
