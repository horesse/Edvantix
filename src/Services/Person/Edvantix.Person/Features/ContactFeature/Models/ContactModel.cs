using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Person.Features.ContactFeature.Models;

[PublicModel("Контакты пользователя", EntityGroupEnum.Personal, requiredAuth: true)]
public sealed class ContactModel : Model<long>
{
    public ContactType Type { get; set; }
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
}
