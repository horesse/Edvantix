using Edvantix.Constants.Other;

namespace Edvantix.Organizational.Features.OrganizationFeature.Models;

public sealed class ContactModel
{
    public ulong Id { get; set; }
    public ulong OrganizationId { get; set; }
    public ContactType Type { get; set; }
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
}
