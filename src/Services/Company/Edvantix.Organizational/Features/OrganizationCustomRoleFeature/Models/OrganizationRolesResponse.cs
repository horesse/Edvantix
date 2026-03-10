namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Models;

/// <summary>
/// Объединённый ответ с системными базовыми ролями и кастомными ролями организации.
/// </summary>
public sealed record OrganizationRolesResponse(
    IReadOnlyList<BaseRoleModel> BaseRoles,
    IReadOnlyList<OrganizationCustomRoleModel> CustomRoles
);
