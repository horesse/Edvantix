using Edvantix.Constants.Other;

namespace Edvantix.Person.Features.ContactFeature.Models;

public sealed class ContactCreateViewModel
{
    public long PersonInfoId { get; set; }
    public ContactType Type { get; set; }
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
}
