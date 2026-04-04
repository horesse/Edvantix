using Edvantix.Constants.Other;
using Edvantix.Persona.Features.Contacts;
using Edvantix.Persona.Features.Educations;
using Edvantix.Persona.Features.EmploymentHistories;
using Edvantix.Persona.Features.Skills;

namespace Edvantix.Persona.Features.Admin.Profiles;

public sealed record AdminProfileDto(
    Guid Id,
    Guid AccountId,
    string FullName,
    string UserName,
    string? AvatarUrl,
    bool IsBlocked,
    DateTime? LastLoginAt
);

public sealed record AdminProfileDetailDto(
    Guid Id,
    Guid AccountId,
    string UserName,
    string FirstName,
    string LastName,
    string? MiddleName,
    Gender Gender,
    DateOnly BirthDate,
    string? Bio,
    string? AvatarUrl,
    bool IsBlocked,
    DateTime? LastLoginAt,
    IReadOnlyList<ContactDto> Contacts,
    IReadOnlyList<EmploymentHistoryDto> EmploymentHistories,
    IReadOnlyList<EducationDto> Educations,
    IReadOnlyList<SkillDto> Skills
);
