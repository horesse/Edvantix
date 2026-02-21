using Edvantix.Constants.Other;

namespace Edvantix.Persona.Features.Profiles;

/// <summary>
/// Запрос на обновление профиля. Принимается как JSON (не form-data).
/// Аватар обновляется отдельным эндпоинтом PUT /profile/avatar.
/// </summary>
public sealed class UpdateProfileRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? MiddleName { get; init; }
    public Gender Gender { get; init; }
    public DateOnly BirthDate { get; init; }

    /// <summary>Полный список контактов. Заменяет все существующие.</summary>
    public IReadOnlyList<ContactRequest> Contacts { get; init; } = [];

    /// <summary>Полный список образования. Заменяет все существующие записи.</summary>
    public IReadOnlyList<EducationRequest> Educations { get; init; } = [];

    /// <summary>Полная история занятости. Заменяет все существующие записи.</summary>
    public IReadOnlyList<EmploymentHistoryRequest> EmploymentHistories { get; init; } = [];
}

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
