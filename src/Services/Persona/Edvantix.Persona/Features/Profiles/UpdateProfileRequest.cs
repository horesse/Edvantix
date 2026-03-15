using Edvantix.Constants.Other;

namespace Edvantix.Persona.Features.Profiles;

public sealed record ContactRequest(ContactType Type, string Value, string? Description = null);

public sealed record EducationRequest(
    DateOnly DateStart,
    string Institution,
    EducationLevel Level,
    string? Specialty = null,
    DateOnly? DateEnd = null
);

public sealed record EmploymentHistoryRequest(
    string Workplace,
    string Position,
    DateTime StartDate,
    DateTime? EndDate = null,
    string? Description = null
);
