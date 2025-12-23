using Edvantix.Chassis.Converter;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Person.Features.ContactFeature.Models;

namespace Edvantix.Person.Features.ContactFeature.Mappers;

public sealed class ContactCreateConverter : ClassConverter<ContactCreateViewModel, Contact>
{
    public override Contact Map(ContactCreateViewModel source)
    {
        return new Contact(source.PersonInfoId, source.Type, source.Value, source.Description);
    }

    public override ContactCreateViewModel Map(Contact source) =>
        throw new NotImplementedException();

    public override void SetProperties(ContactCreateViewModel source, Contact target) =>
        throw new NotImplementedException();
}
