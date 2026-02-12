using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;

namespace Edvantix.ProfileService.Features.EducationFeature.Models;

[PublicModel("Образование", EntityGroupEnum.Personal, true)]
public sealed class EducationModel
{
    public DateOnly DateStart { get; set; }
    public DateOnly? DateEnd { get; set; }

    public string Institution { get; set; } = null!;
    public string? Specialty { get; set; }
    public EducationLevel EducationLevel { get; set; }
}
