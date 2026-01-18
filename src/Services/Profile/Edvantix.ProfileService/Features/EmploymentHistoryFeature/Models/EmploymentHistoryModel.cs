using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.ProfileService.Features.EmploymentHistoryFeature.Models;

[PublicModel("История трудоустройства", EntityGroupEnum.Personal, requiredAuth: true)]
public sealed class EmploymentHistoryModel : Model<long>
{
    public string CompanyName { get; set; } = null!;
    public string Position { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
}
