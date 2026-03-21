using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Chassis.Security.Authorization;

/// <summary>
/// Authorization requirement that checks whether the authenticated user
/// holds a specific permission in the current school via Organizations gRPC.
/// </summary>
/// <param name="permission">The permission string to check (e.g., "scheduling.create-slot").</param>
public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    /// <summary>Gets the permission string to verify.</summary>
    public string Permission { get; } = permission;
}
