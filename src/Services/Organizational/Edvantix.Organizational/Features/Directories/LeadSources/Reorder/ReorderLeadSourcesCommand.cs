using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate.Specifications;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.LeadSources.Reorder;

/// <summary>Переупорядочить источники привлечения согласно переданному списку идентификаторов.</summary>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record ReorderLeadSourcesCommand(IReadOnlyList<Guid> OrderedIds) : ICommand;

internal sealed class ReorderLeadSourcesCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    ILeadSourceRepository repository
) : ICommandHandler<ReorderLeadSourcesCommand>
{
    public async ValueTask<Unit> Handle(
        ReorderLeadSourcesCommand command,
        CancellationToken cancellationToken
    )
    {
        var items = await repository.ListAsync(
            new LeadSourceReorderSpec(tenantContext.OrganizationId),
            cancellationToken
        );

        var lookup = items.ToDictionary(ls => ls.Id);
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
