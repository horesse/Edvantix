namespace Edvantix.Organizational.Features.GroupMemberFeature.Models;

public sealed class GroupMemberModel
{
    public Guid Id { get; set; }
    public long GroupId { get; set; }
    public long ProfileId { get; set; }
    public GroupRole Role { get; set; }
    public DateTime JoinedAt { get; set; }

    // TODO: Fetch user profile data from Profile service via gRPC
    public string? DisplayName { get; set; }
}
