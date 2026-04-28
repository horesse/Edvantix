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
    LegalForm LegalForm,
    DateOnly RegistrationDate,
    ContactType ContactType,
    string ContactValue,
    string ContactDescription
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
            throw new ForbiddenException("Нет прав.");

        var organization = await repository.GetByIdAsync(command.Id, cancellationToken);
        Guard.Against.NotFound(organization, command.Id);

        organization.Update(
            command.FullLegalName,
            command.ShortName,
            command.OrganizationType,
            command.LegalForm,
            command.RegistrationDate
        );

        organization.UpdatePrimaryContact(
            command.ContactType,
            command.ContactValue,
            command.ContactDescription
        );

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
