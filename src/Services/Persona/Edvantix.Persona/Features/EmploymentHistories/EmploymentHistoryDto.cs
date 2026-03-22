namespace Edvantix.Persona.Features.EmploymentHistories;

public sealed record EmploymentHistoryDto(
    string Workplace,
    string Position,
    DateTime StartDate,
    DateTime? EndDate,
    string? Description
);
