using Edvantix.Constants.Other;

namespace Edvantix.Persona.Features.Profiles;

/// <summary>
/// Полное представление профиля, включая контакты, образование, опыт работы и навыки.
/// Возвращается эндпоинтами /profile/details и /profiles/{id}/details.
/// </summary>
public sealed record ProfileDetailsModel(
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
    IReadOnlyList<ContactModel> Contacts,
    IReadOnlyList<EmploymentHistoryModel> EmploymentHistories,
    IReadOnlyList<EducationModel> Educations,
    IReadOnlyList<SkillModel> Skills
);

/// <summary>Контактные данные пользователя (телефон, email, соц. сети и т.д.).</summary>
public sealed record ContactModel(ContactType Type, string Value, string? Description);

/// <summary>Запись об опыте работы.</summary>
public sealed record EmploymentHistoryModel(
    string Workplace,
    string Position,
    DateTime StartDate,
    DateTime? EndDate,
    string? Description
);

/// <summary>Запись об образовании.</summary>
public sealed record EducationModel(
    DateOnly DateStart,
    DateOnly? DateEnd,
    string Institution,
    string? Specialty,
    EducationLevel EducationLevel
);

/// <summary>Навык из глобального каталога, связанный с профилем.</summary>
public sealed record SkillModel(Guid Id, string Name);
