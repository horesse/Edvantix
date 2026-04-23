using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Features.OrganizationMembers;

public sealed class OrganizationMemberDtoMapper : Mapper<OrganizationMember, OrganizationMemberDto>
{
    public override OrganizationMemberDto Map(OrganizationMember source) =>
        new(source.Id, source.ProfileId, source.Role?.Code ?? string.Empty, source.Status);
}
