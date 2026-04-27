using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

namespace Edvantix.Organizational.Features.Organizations;

public sealed class OrganizationDtoMapper : Mapper<Organization, OrganizationDto>
{
    public override OrganizationDto Map(Organization source) =>
        new(
            source.Id,
            source.FullLegalName,
            source.ShortName,
            source.OrganizationType,
            source.Status,
            source.IsLegalEntity
        );
}

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
            [.. source.Contacts.Select(MapContact)],
            source.LastModifiedAt
        );

    private static ContactDto MapContact(Contact c) =>
        new(c.Id, c.Value, c.Description, c.ContactType, c.IsPrimary);
}
