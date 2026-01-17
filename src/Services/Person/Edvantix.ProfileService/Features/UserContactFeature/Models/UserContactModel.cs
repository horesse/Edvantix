using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.ProfileService.Features.UserContactFeature.Models;

[PublicModel("Контакты пользователя", EntityGroupEnum.Personal, requiredAuth: true)]
public sealed class UserContactModel : Model<long>
{
    public ContactType Type { get; set; }
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
}
