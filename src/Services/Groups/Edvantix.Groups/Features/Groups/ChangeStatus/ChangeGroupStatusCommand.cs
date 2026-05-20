using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Groups.Domain.Permissions;

namespace Edvantix.Groups.Features.Groups.ChangeStatus;

[Transactional]
[RequirePermission(GroupPermissions.Edit)]
public sealed record ChangeGroupStatusCommand(Guid Id, GroupStatus NewStatus) : ICommand;

internal sealed class ChangeGroupStatusCommandHandler(
    ITenantContext tenantContext,
    IGroupRepository repository
) : ICommandHandler<ChangeGroupStatusCommand>
{
    public async ValueTask<Unit> Handle(
        ChangeGroupStatusCommand command,
        CancellationToken cancellationToken
    )
    {
        var group = await repository.GetByIdAsync(command.Id, cancellationToken);
        Guard.Against.NotFound(group, command.Id);

        if (group.OrganizationId != tenantContext.OrganizationId)
            throw new ForbiddenException("Группа не принадлежит текущей организации.");

        group.ChangeStatus(command.NewStatus);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
