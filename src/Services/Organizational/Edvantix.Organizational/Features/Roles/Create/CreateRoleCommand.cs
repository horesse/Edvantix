using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Roles.Create;

[Transactional]
[RequirePermission(OrganizationPermissions.Roles)]
public sealed record CreateRoleCommand(string Name, string? Description) : ICommand<Guid>;

internal sealed class CreateRoleCommandHandler(
    ITenantContext tenantContext,
    IOrganizationRoleRepository repository
) : ICommandHandler<CreateRoleCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        var role = new OrganizationRole(
            tenantContext.OrganizationId,
            command.Name,
            command.Description
        );

        await repository.AddAsync(role, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return role.Id;
    }
}
