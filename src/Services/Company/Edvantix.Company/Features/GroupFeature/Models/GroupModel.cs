namespace Edvantix.Company.Features.GroupFeature.Models;

public sealed class GroupModel
{
    public long Id { get; set; }
    public long OrganizationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int MembersCount { get; set; }
}
