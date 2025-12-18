using Edvantix.Chassis.Converter;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.ContactAggregate;
using Edvantix.OrganizationManagement.Features.Contact.Models;

namespace Edvantix.OrganizationManagement.Features.Contact.Mappers;

public sealed class ContactConverter
    : ClassConverter<ContactModel, Domain.AggregatesModel.ContactAggregate.Contact>
{
    public override Domain.AggregatesModel.ContactAggregate.Contact Map(ContactModel source)
    {
        return new Domain.AggregatesModel.ContactAggregate.Contact(
            source.OrganizationId,
            source.Type,
            source.Value,
            source.Description
        );
    }

    public override ContactModel Map(Domain.AggregatesModel.ContactAggregate.Contact source)
    {
        return new ContactModel
        {
            Id = source.Id,
            OrganizationId = source.OrganizationId,
            Type = source.Type,
            Value = source.Value,
            Description = source.Description,
        };
    }

    public override void SetProperties(
        ContactModel source,
        Domain.AggregatesModel.ContactAggregate.Contact target
    )
    {
        target.UpdateValue(source.Value);
        target.UpdateType(source.Type);
        target.UpdateDescription(source.Description);
    }
}
