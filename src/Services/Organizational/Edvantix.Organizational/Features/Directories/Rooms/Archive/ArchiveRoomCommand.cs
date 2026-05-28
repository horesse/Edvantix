using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Rooms.Archive;

/// <summary>Запрос на архивацию кабинета.</summary>
/// <param name="Id">Идентификатор записи.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record ArchiveRoomCommand(Guid Id) : ICommand;

internal sealed class ArchiveRoomCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IRoomRepository repository
) : ICommandHandler<ArchiveRoomCommand>
{
    public async ValueTask<Unit> Handle(
        ArchiveRoomCommand command,
        CancellationToken cancellationToken
    )
    {
        var room = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (room is null || room.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Room>(command.Id);

        var by = claims.GetProfileIdOrError();

        room.Archive(by);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
