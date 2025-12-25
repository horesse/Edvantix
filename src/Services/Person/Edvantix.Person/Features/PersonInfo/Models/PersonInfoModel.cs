using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Person.Features.PersonInfo.Models;

[PublicModel("Персональная информация", EntityGroupEnum.Personal, requiredAuth: true)]
public sealed class PersonInfoModel : Model<long>
{
    public Guid AccountId { get; set; }
    public Gender Gender { get; set; }
}
