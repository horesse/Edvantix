namespace Edvantix.Organizational.Features.GroupFeature.Models;

public sealed class GroupModel
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int MembersCount { get; set; }
}
