using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.Organization.Domain.AggregatesModel.ContactAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organization.Features.Contact.Models;

[PublicModel(
    desc: "Контакт организации",
    entityType: EntityGroupEnum.Reference,
    requiredAuth: true
)]
public sealed class ContactModel : Model<long>
{
    public long OrganizationId { get; set; }
    public ContactType Type { get; set; }
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
}
