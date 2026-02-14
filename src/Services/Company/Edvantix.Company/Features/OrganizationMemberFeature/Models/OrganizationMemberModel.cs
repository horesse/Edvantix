using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Company.Features.OrganizationMemberFeature.Models;

public sealed class OrganizationMemberModel
{
    public Guid Id { get; set; }
    public long OrganizationId { get; set; }
    public long ProfileId { get; set; }
    public OrganizationRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    // TODO: Fetch user profile data from Profile service via gRPC
    public string? DisplayName { get; set; }
}
