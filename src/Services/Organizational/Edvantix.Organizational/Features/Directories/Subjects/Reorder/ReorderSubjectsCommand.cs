using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.SubjectAggregate.Specifications;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Subjects.Reorder;

/// <summary>Переупорядочить предметы согласно переданному списку идентификаторов.</summary>
[Transactional]
[RequirePermission(SubjectPermissions.Manage)]
public sealed record ReorderSubjectsCommand(IReadOnlyList<Guid> OrderedIds) : ICommand;

internal sealed class ReorderSubjectsCommandHandler(
    ITenantContext tenantContext,
    ISubjectRepository repository
) : ICommandHandler<ReorderSubjectsCommand>
{
    public async ValueTask<Unit> Handle(
        ReorderSubjectsCommand command,
        CancellationToken cancellationToken
    )
    {
        var items = await repository.ListAsync(
            new SubjectReorderSpec(tenantContext.OrganizationId),
            cancellationToken
        );

        var lookup = items.ToDictionary(s => s.Id);

        for (var i = 0; i < command.OrderedIds.Count; i++)
        {
            if (lookup.TryGetValue(command.OrderedIds[i], out var item))
                item.SetOrder(i, Guid.Empty);
        }

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
