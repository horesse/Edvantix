using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Features.Roles;

public sealed class RoleDtoMapper : Mapper<OrganizationMemberRole, RoleDto>
{
    public override RoleDto Map(OrganizationMemberRole source) =>
        new(source.Id, source.OrganizationId, source.Code, source.Description);
}

public sealed class RoleDetailDtoMapper : Mapper<OrganizationMemberRole, RoleDetailDto>
{
    public override RoleDetailDto Map(OrganizationMemberRole source) =>
        new(
            source.Id,
            source.OrganizationId,
            source.Code,
            source.Description,
            source
                .Permissions.Select(p => new PermissionDto(
                    p.Id,
                    new FeatureDto(p.FeatureCode, p.FeatureName),
                    p.Code,
                    p.Name
                ))
                .ToList()
        );
}
