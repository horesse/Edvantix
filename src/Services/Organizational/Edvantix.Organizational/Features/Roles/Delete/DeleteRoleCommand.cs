using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Roles.Delete;

[Transactional]
[RequirePermission(OrganizationPermissions.Roles)]
public sealed record DeleteRoleCommand(Guid Id) : ICommand;

internal sealed class DeleteRoleCommandHandler(
    ITenantContext tenantContext,
    IOrganizationRoleRepository repository
) : ICommandHandler<DeleteRoleCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        var role = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (role is null || role.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<OrganizationRole>(command.Id);

        role.Delete();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
