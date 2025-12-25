using Edvantix.Chassis.Converter;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Person.Features.ContactFeature.Models;

namespace Edvantix.Person.Features.ContactFeature.Mappers;

public sealed class ContactConverter : ClassConverter<ContactModel, Contact>
{
    public override Contact Map(ContactModel source) =>
        new(source.Type, source.Value, source.Description);

    public override ContactModel Map(Contact source) =>
        new()
        {
            Type = source.Type,
            Value = source.Value,
            Description = source.Description,
        };

    public override void SetProperties(ContactModel source, Contact target)
    {
        target.UpdateDescription(source.Description);
        target.UpdateType(source.Type);
        target.UpdateValue(source.Value);
    }
}
