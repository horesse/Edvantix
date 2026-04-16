using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Features.Organizations;

public sealed record OrganizationDto(
    Guid Id,
    string FullLegalName,
    string? ShortName,
    OrganizationType OrganizationType,
    OrganizationStatus Status,
    bool IsLegalEntity
);

public sealed record OrganizationDetailDto(
    Guid Id,
    string FullLegalName,
    string? ShortName,
    bool IsLegalEntity,
    DateOnly RegistrationDate,
    LegalForm LegalForm,
    Guid CountryId,
    Guid CurrencyId,
    OrganizationType OrganizationType,
    OrganizationStatus Status,
    IReadOnlyList<ContactDto> Contacts
);

public sealed record ContactDto(
    Guid Id,
    string Value,
    string Description,
    ContactType ContactType,
    bool IsPrimary
);

public sealed class OrganizationDetailDtoMapper : Mapper<Organization, OrganizationDetailDto>
{
    public override OrganizationDetailDto Map(Organization source) =>
        new(
            source.Id,
            source.FullLegalName,
            source.ShortName,
            source.IsLegalEntity,
            source.RegistrationDate,
            source.LegalForm,
            source.CountryId,
            source.CurrencyId,
            source.OrganizationType,
            source.Status,
            source.Contacts.Where(c => !c.IsDeleted).Select(MapContact).ToList()
        );

    private static ContactDto MapContact(Contact c) =>
        new(c.Id, c.Value, c.Description, c.ContactType, c.IsPrimary);
}
