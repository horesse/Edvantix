namespace Edvantix.Organizational.Features.OrganizationMemberFeature.Models;

public sealed class OrganizationMemberModel
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid ProfileId { get; set; }
    public OrganizationRole Role { get; set; }
    public DateTime JoinedAt { get; set; }

    // TODO: Fetch user profile data from Profile service via gRPC
    public string? DisplayName { get; set; }
}
