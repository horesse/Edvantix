using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;

namespace Edvantix.ProfileService.Features.EducationFeature.Models;

[PublicModel("Образование", EntityGroupEnum.Personal, true)]
public sealed class EducationModel
{
    public DateTime DateStart { get; set; }
    public DateTime? DateEnd { get; set; }

    public string Institution { get; set; } = null!;
    public string? Specialty { get; set; }
    public EducationLevel EducationLevel { get; set; }
}
