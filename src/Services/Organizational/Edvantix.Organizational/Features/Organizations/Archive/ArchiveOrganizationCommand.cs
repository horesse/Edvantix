using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.Organizations.Archive;

[Transactional]
[RequirePermission(OrganizationPermissions.Update)]
public sealed record ArchiveOrganizationCommand(Guid Id) : ICommand;

internal sealed class ArchiveOrganizationCommandHandler(
    IOrganizationRepository repository,
    ITenantContext tenantContext
) : ICommandHandler<ArchiveOrganizationCommand>
{
    public async ValueTask<Unit> Handle(
        ArchiveOrganizationCommand command,
        CancellationToken cancellationToken
    )
    {
        if (tenantContext.OrganizationId != command.Id)
            throw new ForbiddenException("Нет прав.");

        var organization = await repository.GetByIdAsync(command.Id, cancellationToken);
        Guard.Against.NotFound(organization, command.Id);

        organization.Archive();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
