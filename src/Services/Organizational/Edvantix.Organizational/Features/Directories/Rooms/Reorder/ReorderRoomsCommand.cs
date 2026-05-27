using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate.Specifications;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.Rooms.Reorder;

/// <summary>Переупорядочить кабинеты согласно переданному списку идентификаторов.</summary>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record ReorderRoomsCommand(IReadOnlyList<Guid> OrderedIds) : ICommand;

internal sealed class ReorderRoomsCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IRoomRepository repository
) : ICommandHandler<ReorderRoomsCommand>
{
    public async ValueTask<Unit> Handle(
        ReorderRoomsCommand command,
        CancellationToken cancellationToken
    )
    {
        var items = await repository.ListAsync(
            new RoomReorderSpec(tenantContext.OrganizationId),
            cancellationToken
        );

        var lookup = items.ToDictionary(r => r.Id);
        var by = claims.GetProfileIdOrError();

        for (var i = 0; i < command.OrderedIds.Count; i++)
        {
            if (lookup.TryGetValue(command.OrderedIds[i], out var item))
                item.SetOrder(i, by);
        }

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
