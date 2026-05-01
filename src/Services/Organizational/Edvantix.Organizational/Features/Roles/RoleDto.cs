namespace Edvantix.Organizational.Features.Roles;

public sealed record RoleDto(Guid Id, Guid OrganizationId, string Code, string? Description);

public sealed record RoleDetailDto(
    Guid Id,
    Guid OrganizationId,
    string Code,
    string? Description,
    IReadOnlyList<PermissionDto> Permissions
);

public sealed record FeatureDto(Guid Id, string Code, string Name);

public sealed record PermissionDto(Guid Id, FeatureDto Feature, string Code, string Name);
