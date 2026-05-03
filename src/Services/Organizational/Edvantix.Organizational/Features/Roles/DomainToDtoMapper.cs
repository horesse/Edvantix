using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;

namespace Edvantix.Organizational.Features.Roles;

public sealed class RoleDtoMapper : Mapper<OrganizationRole, RoleDto>
{
    public override RoleDto Map(OrganizationRole source) =>
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

public sealed class RoleDetailDtoMapper : Mapper<OrganizationRole, RoleDetailDto>
{
    public override RoleDetailDto Map(OrganizationRole source) =>
        new(
            source.Id,
            source.OrganizationId,
            source.Name,
            source.Description,
            source.IsSystem,
            source.IsOwner
        );
}
