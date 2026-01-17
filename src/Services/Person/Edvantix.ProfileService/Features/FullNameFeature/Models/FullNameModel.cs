using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.ProfileService.Features.FullNameFeature.Models;

[PublicModel("ФИО", EntityGroupEnum.Personal, requiredAuth: true)]
public sealed class FullNameModel : Model<long>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
}
