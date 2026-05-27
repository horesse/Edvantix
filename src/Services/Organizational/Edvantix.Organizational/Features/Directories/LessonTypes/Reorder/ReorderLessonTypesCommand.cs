using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.LessonTypeAggregate;
using Edvantix.Organizational.Domain.LessonTypeAggregate.Specifications;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.LessonTypes.Reorder;

/// <summary>Переупорядочить типы занятий согласно переданному списку идентификаторов.</summary>
[Transactional]
[RequirePermission(LessonTypePermissions.Manage)]
public sealed record ReorderLessonTypesCommand(IReadOnlyList<Guid> OrderedIds) : ICommand;

internal sealed class ReorderLessonTypesCommandHandler(
    ITenantContext tenantContext,
    ILessonTypeRepository repository
) : ICommandHandler<ReorderLessonTypesCommand>
{
    public async ValueTask<Unit> Handle(
        ReorderLessonTypesCommand command,
        CancellationToken cancellationToken
    )
    {
        var items = await repository.ListAsync(
            new LessonTypeReorderSpec(tenantContext.OrganizationId),
            cancellationToken
        );

        var lookup = items.ToDictionary(lt => lt.Id);

        for (var i = 0; i < command.OrderedIds.Count; i++)
        {
            if (lookup.TryGetValue(command.OrderedIds[i], out var item))
                item.SetOrder(i, Guid.Empty);
        }

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
