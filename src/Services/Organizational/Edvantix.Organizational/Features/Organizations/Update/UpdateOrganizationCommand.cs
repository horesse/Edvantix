using System.Security.Claims;
using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.Utilities;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Organizations.Update;

[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
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
    ITenantContext tenantContext,
    ClaimsPrincipal claims
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

        var modifiedBy = claims.GetProfileIdOrError();

        organization.Update(
            command.FullLegalName,
            command.ShortName,
            command.OrganizationType,
            command.LegalForm,
            command.RegistrationDate,
            modifiedBy
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
