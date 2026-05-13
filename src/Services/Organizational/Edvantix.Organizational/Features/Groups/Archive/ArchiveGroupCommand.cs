using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Groups.Archive;

[Transactional]
[RequirePermission(GroupPermissions.Edit)]
public sealed record ArchiveGroupCommand(Guid Id) : ICommand;

internal sealed class ArchiveGroupCommandHandler(
    ITenantContext tenantContext,
    IGroupRepository repository
) : ICommandHandler<ArchiveGroupCommand>
{
    public async ValueTask<Unit> Handle(
        ArchiveGroupCommand command,
        CancellationToken cancellationToken
    )
    {
        var group = await repository.GetByIdAsync(command.Id, cancellationToken);
        Guard.Against.NotFound(group, command.Id);

        if (group.OrganizationId != tenantContext.OrganizationId)
            throw new ForbiddenException("Группа не принадлежит текущей организации.");

        group.Archive();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
