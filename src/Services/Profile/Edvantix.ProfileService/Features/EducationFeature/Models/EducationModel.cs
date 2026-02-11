using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;

namespace Edvantix.ProfileService.Features.EducationFeature.Models;

[PublicModel("Образование", EntityGroupEnum.Personal, true)]
public sealed class EducationModel
{
    public DateTime DateStart { get; set; }
    public DateTime? DateEnd { get; set; }

    public string Institution { get; set; } = null!;
    public string? Specialty { get; set; }
    public long EducationLevelId { get; set; }
    public string EducationLevel { get; set; } = null!;
}
