using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.Rooms.Restore;

/// <summary>Запрос на восстановление кабинета из архива.</summary>
/// <param name="Id">Идентификатор записи.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record RestoreRoomCommand(Guid Id) : ICommand;

internal sealed class RestoreRoomCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IRoomRepository repository
) : ICommandHandler<RestoreRoomCommand>
{
    public async ValueTask<Unit> Handle(
        RestoreRoomCommand command,
        CancellationToken cancellationToken
    )
    {
        var room = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (room is null || room.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Room>(command.Id);

        var by = claims.GetProfileIdOrError();

        room.Restore(by);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
