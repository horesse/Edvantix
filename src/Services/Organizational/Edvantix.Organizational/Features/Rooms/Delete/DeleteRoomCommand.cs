using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Rooms.Delete;

[Transactional]
[RequirePermission(OrganizationPermissions.Rooms)]
public sealed record DeleteRoomCommand(Guid Id) : ICommand;

internal sealed class DeleteRoomCommandHandler(
    ITenantContext tenantContext,
    IRoomRepository repository
) : ICommandHandler<DeleteRoomCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteRoomCommand command,
        CancellationToken cancellationToken
    )
    {
        var room = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (room is null || room.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Room>(command.Id);

        room.Delete();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
