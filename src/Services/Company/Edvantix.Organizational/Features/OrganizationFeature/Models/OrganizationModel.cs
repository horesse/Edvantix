namespace Edvantix.Organizational.Features.OrganizationFeature.Models;

public sealed class OrganizationModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string NameLatin { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string? PrintName { get; set; }
    public string? Description { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int MembersCount { get; set; }
    public int GroupsCount { get; set; }
}
