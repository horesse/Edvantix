using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.ProfileService.Features.ProfileFeature.Models;

[PublicModel("Персональная информация", EntityGroupEnum.Personal, requiredAuth: true)]
public sealed class ProfileModel : Model<long>
{
    public Guid AccountId { get; set; }
    public Gender Gender { get; set; }

    public DateOnly BirthDate { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
}
