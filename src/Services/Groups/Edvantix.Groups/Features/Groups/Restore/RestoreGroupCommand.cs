using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Groups.Restore;

[Transactional]
[RequirePermission(GroupPermissions.Edit)]
public sealed record RestoreGroupCommand(Guid Id) : ICommand;

internal sealed class RestoreGroupCommandHandler(
    ITenantContext tenantContext,
    IGroupRepository repository
) : ICommandHandler<RestoreGroupCommand>
{
    public async ValueTask<Unit> Handle(
        RestoreGroupCommand command,
        CancellationToken cancellationToken
    )
    {
        var group = await repository.GetByIdAsync(command.Id, cancellationToken);
        Guard.Against.NotFound(group, command.Id);

        if (group.OrganizationId != tenantContext.OrganizationId)
            throw new ForbiddenException("Группа не принадлежит текущей организации.");

        group.Restore();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
