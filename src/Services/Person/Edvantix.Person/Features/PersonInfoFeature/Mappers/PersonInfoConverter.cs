using Edvantix.Chassis.Converter;
using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;
using Edvantix.Person.Features.PersonInfoFeature.Models;

namespace Edvantix.Person.Features.PersonInfoFeature.Mappers;

public sealed class PersonInfoConverter : ClassConverter<PersonInfoModel, PersonInfo>
{
    public override PersonInfo Map(PersonInfoModel source) => throw new NotImplementedException();

    public override PersonInfoModel Map(PersonInfo source) =>
        new()
        {
            AccountId = source.AccountId,
            FirstName = source.FullName.FirstName,
            LastName = source.FullName.LastName,
            MiddleName = source.FullName.MiddleName,
            Gender = source.Gender,
            Id = source.Id,
        };

    public override void SetProperties(PersonInfoModel source, PersonInfo target)
    {
        target.UpdateGender(source.Gender);
    }
}
