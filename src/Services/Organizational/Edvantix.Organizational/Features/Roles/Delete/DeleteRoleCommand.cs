using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.Roles.Delete;

[Transactional]
[RequirePermission(OrganizationPermissions.ManageRoles)]
public sealed record DeleteRoleCommand(Guid Id) : ICommand;

internal sealed class DeleteRoleCommandHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRoleRepository repository
) : ICommandHandler<DeleteRoleCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        var role = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (role is null || role.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<OrganizationMemberRole>(command.Id);

        role.Delete();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
