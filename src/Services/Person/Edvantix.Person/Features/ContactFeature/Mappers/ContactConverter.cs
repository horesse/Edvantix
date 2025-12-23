using Edvantix.Chassis.Converter;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Person.Features.ContactFeature.Models;

namespace Edvantix.Person.Features.ContactFeature.Mappers;

public sealed class ContactConverter : ClassConverter<ContactModel, Contact>
{
    public override Contact Map(ContactModel source) => throw new NotImplementedException();

    public override ContactModel Map(Contact source)
    {
        return new ContactModel
        {
            Type = source.Type,
            Value = source.Value,
            Description = source.Description,
        };
    }

    public override void SetProperties(ContactModel source, Contact target)
    {
        target.UpdateDescription(source.Description);
        target.UpdateType(source.Type);
        target.UpdateValue(source.Value);
    }
}
