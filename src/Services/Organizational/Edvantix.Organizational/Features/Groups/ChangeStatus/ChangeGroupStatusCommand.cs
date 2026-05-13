using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Groups.ChangeStatus;

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
