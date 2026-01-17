using Edvantix.Chassis.Converter;
using Edvantix.ProfileService.Domain.AggregatesModel.ContactAggregate;
using Edvantix.ProfileService.Features.UserContactFeature.Models;

namespace Edvantix.ProfileService.Features.UserContactFeature.Mappers;

public sealed class UserContactConverter : ClassConverter<UserContactModel, UserContact>
{
    public override UserContact Map(UserContactModel source) =>
        new(source.Type, source.Value, source.Description);

    public override UserContactModel Map(UserContact source) =>
        new()
        {
            Type = source.Type,
            Value = source.Value,
            Description = source.Description,
            Id = source.Id,
        };

    public override void SetProperties(UserContactModel source, UserContact target)
    {
        target.UpdateDescription(source.Description);
        target.UpdateType(source.Type);
        target.UpdateValue(source.Value);
    }
}
