using Edvantix.Chassis.Converter;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.ProfileService.Features.ProfileFeature.Models;

namespace Edvantix.ProfileService.Features.ProfileFeature.Mappers;

public sealed class ProfileConverter
    : ClassConverter<ProfileModel, Profile>
{
    public override Profile Map(ProfileModel source) =>
        throw new NotImplementedException();

    public override ProfileModel Map(Profile source) =>
        new()
        {
            AccountId = source.AccountId,
            FirstName = source.FullName.FirstName,
            LastName = source.FullName.LastName,
            MiddleName = source.FullName.MiddleName,
            Gender = source.Gender,
            Id = source.Id,
        };

    public override void SetProperties(
        ProfileModel source,
        Profile target
    )
    {
        target.UpdateGender(source.Gender);
    }
}
