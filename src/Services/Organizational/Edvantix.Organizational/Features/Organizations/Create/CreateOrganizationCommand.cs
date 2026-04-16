using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Features.Organizations.Create;

public sealed record CreateOrganizationCommand(
    string FullLegalName,
    string? ShortName,
    bool IsLegalEntity,
    DateOnly RegistrationDate,
    LegalForm LegalForm,
    Guid CountryId,
    Guid CurrencyId,
    OrganizationType OrganizationType,
    string PrimaryContactValue,
    ContactType PrimaryContactType,
    string PrimaryContactDescription
) : ICommand<Guid>;

internal sealed class CreateOrganizationCommandHandler(
    ClaimsPrincipal claims,
    IOrganizationRepository repository
) : ICommandHandler<CreateOrganizationCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken
    )
    {
        var ownerProfileId = claims.GetProfileIdOrError();

        var organization = new Organization(
            command.FullLegalName,
            command.IsLegalEntity,
            command.RegistrationDate,
            command.LegalForm,
            command.CountryId,
            command.CurrencyId,
            command.OrganizationType,
            command.ShortName
        );

        var contact = new Contact(
            organization.Id,
            command.PrimaryContactValue,
            command.PrimaryContactDescription,
            command.PrimaryContactType,
            isPrimary: true
        );
        organization.AddContact(contact);

        organization.InitializeOwnership(ownerProfileId);

        await repository.AddAsync(organization, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return organization.Id;
    }
}
