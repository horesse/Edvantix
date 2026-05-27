using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentTags.Reorder;

/// <summary>Переупорядочить теги студентов согласно переданному списку идентификаторов.</summary>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record ReorderStudentTagsCommand(IReadOnlyList<Guid> OrderedIds) : ICommand;

internal sealed class ReorderStudentTagsCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IStudentTagRepository repository
) : ICommandHandler<ReorderStudentTagsCommand>
{
    public async ValueTask<Unit> Handle(
        ReorderStudentTagsCommand command,
        CancellationToken cancellationToken
    )
    {
        var items = await repository.ListAsync(
            new StudentTagReorderSpec(tenantContext.OrganizationId),
            cancellationToken
        );

        var lookup = items.ToDictionary(t => t.Id);
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
