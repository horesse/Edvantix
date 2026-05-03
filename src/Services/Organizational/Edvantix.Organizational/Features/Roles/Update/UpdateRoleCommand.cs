using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Roles.Update;

[Transactional]
[RequirePermission(OrganizationPermissions.Roles)]
public sealed record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<Guid> PermissionIds
) : ICommand;

internal sealed class UpdateRoleCommandHandler(
    ITenantContext tenantContext,
    IOrganizationRoleRepository repository,
    IPermissionRepository permissionRepository
) : ICommandHandler<UpdateRoleCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        var role = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (role is null || role.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<OrganizationRole>(command.Id);

        role.Update(command.Name, command.Description);

        if (!role.IsOwner)
        {
            var permissions = await permissionRepository.GetByIdsAsync(
                command.PermissionIds,
                cancellationToken
            );
            role.AssignPermissions(permissions);
        }

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
