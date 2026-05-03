using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Features.Roles;

public sealed class RoleDtoMapper : Mapper<OrganizationMemberRole, RoleDto>
{
    public override RoleDto Map(OrganizationMemberRole source) =>
        new(
            source.Id,
            source.OrganizationId,
            source.Name,
            source.Description,
            source.IsSystem,
            source.IsOwner,
            source.Permissions.Count
        );
}

public sealed class RoleDetailDtoMapper : Mapper<OrganizationMemberRole, RoleDetailDto>
{
    public override RoleDetailDto Map(OrganizationMemberRole source) =>
        new(
            source.Id,
            source.OrganizationId,
            source.Name,
            source.Description,
            source.IsSystem,
            source.IsOwner
        );
}
