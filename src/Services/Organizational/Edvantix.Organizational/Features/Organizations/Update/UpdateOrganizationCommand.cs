using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Features.Organizations.Update;

[Transactional]
[RequirePermission(OrganizationPermissions.Update)]
public sealed record UpdateOrganizationCommand(
    Guid Id,
    string FullLegalName,
    string? ShortName,
    OrganizationType OrganizationType,
    LegalForm LegalForm
) : ICommand;

internal sealed class UpdateOrganizationCommandHandler(
    IOrganizationRepository repository,
    ITenantContext tenantContext
) : ICommandHandler<UpdateOrganizationCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateOrganizationCommand command,
        CancellationToken cancellationToken
    )
    {
        if (tenantContext.OrganizationId != command.Id)
            throw new ForbiddenException(
                "Попытка обновить учетную отличную от активной организации."
            );

        var organization = await repository.GetByIdAsync(command.Id, cancellationToken);
        Guard.Against.NotFound(organization, command.Id);

        organization.Update(
            command.FullLegalName,
            command.ShortName,
            command.OrganizationType,
            command.LegalForm
        );

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
