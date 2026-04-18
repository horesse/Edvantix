using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.Organizations.Delete;

[Transactional]
[RequirePermission(OrganizationPermissions.Delete)]
public sealed record DeleteOrganizationCommand(Guid Id) : ICommand;

internal sealed class DeleteOrganizationCommandHandler(IOrganizationRepository repository)
    : ICommandHandler<DeleteOrganizationCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteOrganizationCommand command,
        CancellationToken cancellationToken
    )
    {
        var organization = await repository.GetByIdAsync(command.Id, cancellationToken);
        Guard.Against.NotFound(organization, command.Id);

        organization.Delete();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
