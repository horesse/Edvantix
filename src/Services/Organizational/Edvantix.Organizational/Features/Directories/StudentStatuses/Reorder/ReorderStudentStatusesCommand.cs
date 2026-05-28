using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate.Specifications;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Reorder;

/// <summary>Переупорядочить статусы студентов согласно переданному списку идентификаторов.</summary>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record ReorderStudentStatusesCommand(IReadOnlyList<Guid> OrderedIds) : ICommand;

internal sealed class ReorderStudentStatusesCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IStudentStatusRepository repository
) : ICommandHandler<ReorderStudentStatusesCommand>
{
    public async ValueTask<Unit> Handle(
        ReorderStudentStatusesCommand command,
        CancellationToken cancellationToken
    )
    {
        var items = await repository.ListAsync(
            new StudentStatusReorderSpec(tenantContext.OrganizationId),
            cancellationToken
        );

        var lookup = items.ToDictionary(s => s.Id);
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
