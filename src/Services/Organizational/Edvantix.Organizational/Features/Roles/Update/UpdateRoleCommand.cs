using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.Roles.Update;

[Transactional]
[RequirePermission(OrganizationPermissions.ManageRoles)]
public sealed record UpdateRoleCommand(Guid Id, string Code, string? Description) : ICommand;

internal sealed class UpdateRoleCommandHandler(
    ITenantContext tenantContext,
    IOrganizationMemberRoleRepository repository
) : ICommandHandler<UpdateRoleCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        var role = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (role is null || role.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<OrganizationMemberRole>(command.Id);

        role.Update(command.Code, command.Description);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
