using Edvantix.Constants.Other;

namespace Edvantix.ProfileService.Features.ProfileFeature.Models;

/// <summary>
/// Модель для обновления профиля пользователя
/// </summary>
public sealed record UpdateProfileModel
{
    public Gender Gender { get; init; }
    public DateOnly BirthDate { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? MiddleName { get; init; }

    /// <summary>
    /// Контакты пользователя. Если указаны, полностью заменяют существующие контакты.
    /// </summary>
    public List<UpdateUserContactModel>? Contacts { get; init; }

    /// <summary>
    /// История трудоустройства. Если указана, полностью заменяет существующую историю.
    /// </summary>
    public List<UpdateEmploymentHistoryModel>? EmploymentHistories { get; init; }

    /// <summary>
    /// Образование. Если указано, полностью заменяет существующее образование.
    /// </summary>
    public List<UpdateEducationModel>? Educations { get; init; }
}
