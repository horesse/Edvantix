namespace Edvantix.Persona.Features.Educations;

public sealed record EducationDto(
    DateOnly DateStart,
    DateOnly? DateEnd,
    string Institution,
    string? Specialty,
    EducationLevel EducationLevel
);
