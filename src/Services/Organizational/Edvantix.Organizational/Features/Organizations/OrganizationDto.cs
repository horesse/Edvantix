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

public sealed record OrganizationWithRoleDto(
    Guid Id,
    string FullLegalName,
    string? ShortName,
    OrganizationType OrganizationType,
    OrganizationStatus Status,
    bool IsLegalEntity,
    string RoleCode,
    string? RoleDescription
);
