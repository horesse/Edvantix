using Edvantix.Constants.Other;
using Edvantix.Persona.Features.Contacts;
using Edvantix.Persona.Features.Educations;
using Edvantix.Persona.Features.EmploymentHistories;
using Edvantix.Persona.Features.Skills;

namespace Edvantix.Persona.Features.Profiles;

public sealed record ProfileDetailsDto(
    Guid Id,
    Guid AccountId,
    string Login,
    Gender Gender,
    DateOnly BirthDate,
    string FirstName,
    string LastName,
    string? MiddleName,
    string? AvatarUrl,
    string? Bio,
    IReadOnlyList<ContactDto> Contacts,
    IReadOnlyList<EmploymentHistoryDto> EmploymentHistories,
    IReadOnlyList<EducationDto> Educations,
    IReadOnlyList<SkillDto> Skills
);
