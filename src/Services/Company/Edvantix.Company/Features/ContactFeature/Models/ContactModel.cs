using Edvantix.Constants.Other;

namespace Edvantix.Company.Features.ContactFeature.Models;

public sealed class ContactModel
{
    public long Id { get; set; }
    public long OrganizationId { get; set; }
    public ContactType Type { get; set; }
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
}
