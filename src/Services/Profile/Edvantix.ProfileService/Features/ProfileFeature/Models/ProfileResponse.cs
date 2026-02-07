using Edvantix.Constants.Other;

namespace Edvantix.ProfileService.Features.ProfileFeature.Models;

/// <summary>
/// Модель контакта для ответа
/// </summary>
public sealed record ContactResponse(ContactType Type, string Value, string? Description);

/// <summary>
/// Модель истории трудоустройства для ответа
/// </summary>
public sealed record EmploymentHistoryResponse(
    string Workplace,
    string Position,
    DateTime StartDate,
    DateTime? EndDate,
    string? Description
);

/// <summary>
/// Модель образования для ответа
/// </summary>
public sealed record EducationResponse(
    DateTime DateStart,
    DateTime? DateEnd,
    string Institution,
    string? Specialty,
    long EducationLevelId,
    string? EducationLevelName
);

/// <summary>
/// Ответ с данными профиля
/// </summary>
public sealed record ProfileResponse(
    long Id,
    Guid AccountId,
    Gender Gender,
    DateOnly BirthDate,
    string FullName,
    string FirstName,
    string LastName,
    string? MiddleName,
    List<ContactResponse>? Contacts = null,
    List<EmploymentHistoryResponse>? EmploymentHistories = null,
    List<EducationResponse>? Educations = null
);
