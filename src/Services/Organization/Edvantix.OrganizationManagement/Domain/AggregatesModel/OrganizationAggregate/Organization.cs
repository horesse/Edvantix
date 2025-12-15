using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.OrganizationAggregate;

public sealed class Organization : LongIdentity, IAggregateRoot
{
    public string Name { get; set; } = null!;
    public string NameLatin { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string? PrintName { get; set; }
    public string? Description { get; set; }
    public DateTime RegistrationDate { get; set; }
}
