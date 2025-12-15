using Edvantix.OrganizationManagement.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.ContactAggregate;

public sealed class Contact : LongIdentity, IAggregateRoot
{
    public long OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public ContactType Type { get; set; }
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
}
