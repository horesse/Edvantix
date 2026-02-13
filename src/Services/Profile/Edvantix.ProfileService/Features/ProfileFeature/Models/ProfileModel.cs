using System.Text.Json.Serialization;
using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.ProfileService.Features.EducationFeature.Models;
using Edvantix.ProfileService.Features.EmploymentHistoryFeature.Models;
using Edvantix.ProfileService.Features.UserContactFeature.Models;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.ProfileService.Features.ProfileFeature.Models;

[PublicModel("Персональная информация", EntityGroupEnum.Personal, requiredAuth: true)]
public sealed class ProfileModel : Model<long>
{
    public Guid AccountId { get; set; }
    public string Login { get; set; } = null!;
    public Gender Gender { get; set; }

    public DateOnly BirthDate { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }

    public IFormFile? Avatar { get; set; } = null!;

    public string? AvatarUrl { get; set; } = null!;

    public IEnumerable<UserContactModel>? Contacts { get; set; }

    public IEnumerable<EmploymentHistoryModel>? EmploymentHistories { get; set; }

    public IEnumerable<EducationModel>? Educations { get; set; }

    [JsonIgnore]
    public string FullName => $"{LastName} {FirstName} {MiddleName}".TrimEnd();
}
