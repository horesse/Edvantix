using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.Roles.Create;

[Transactional]
[RequirePermission(OrganizationPermissions.ManageRoles)]
public sealed record CreateRoleCommand(string Code, string? Description) : ICommand<Guid>;

internal sealed class CreateRoleCommandHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRoleRepository repository
) : ICommandHandler<CreateRoleCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        var role = new OrganizationMemberRole(
            tenantContext.OrganizationId,
            command.Code,
            command.Description
        );

        await repository.AddAsync(role, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return role.Id;
    }
}
