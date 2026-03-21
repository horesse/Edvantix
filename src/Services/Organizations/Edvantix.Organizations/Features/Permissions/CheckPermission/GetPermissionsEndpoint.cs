namespace Edvantix.Organizations.Features.Permissions.CheckPermission;

/// <summary>
/// GET /v1/permissions?userId={id}&amp;schoolId={id} — returns all permission strings for a user in a school.
///
/// Purpose: Cache priming for downstream services. Called during startup to populate the
/// HybridCache with a user's full permission set before individual CheckPermission calls are made.
///
/// This endpoint queries the database directly (no HybridCache) to return the authoritative
/// permission list.
///
/// Allowed anonymous: same convention as RegisterPermissionsEndpoint — called by service
/// identities, not end users.
/// TODO: In production, secure this endpoint with a shared secret or service-to-service mTLS policy.
/// </summary>
public sealed class GetPermissionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/permissions",
                async (
                    Guid userId,
                    Guid schoolId,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await sender.Send(
                        new GetUserPermissionsQuery(userId, schoolId),
                        cancellationToken
                    );

                    return TypedResults.Ok(result);
                }
            )
            .Produces<string[]>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetPermissions")
            .WithTags("Permissions")
            .WithSummary("Get user permissions")
            .WithDescription(
                "Returns all permission strings for the specified user within a school. "
                    + "Used by downstream services to prime their local permission cache. "
                    + "Queries the database directly — not cached."
            )
            .MapToApiVersion(ApiVersions.V1)
            .AllowAnonymous();
    }
}

/// <summary>Query that returns all permission strings for a user in a school, bypassing the cache.</summary>
/// <param name="UserId">The user profile identifier.</param>
/// <param name="SchoolId">The school identifier.</param>
public sealed record GetUserPermissionsQuery(Guid UserId, Guid SchoolId) : IQuery<string[]>;

/// <summary>
/// Loads role assignments for the user+school, fetches the assigned roles,
/// resolves permission names from the global catalogue, and returns a distinct sorted list.
/// Bypasses tenant query filters — uses explicit schoolId filtering.
/// </summary>
public sealed class GetUserPermissionsQueryHandler(
    IUserRoleAssignmentRepository assignmentRepository,
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository
) : IQueryHandler<GetUserPermissionsQuery, string[]>
{
    /// <inheritdoc/>
    public async ValueTask<string[]> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var assignments = await assignmentRepository.GetByProfileAndSchoolAsync(
            request.UserId,
            request.SchoolId,
            cancellationToken
        );

        if (assignments.Count == 0)
        {
            return [];
        }

        var roleIds = assignments.Select(a => a.RoleId).ToHashSet();

        var roles = await roleRepository.GetBySchoolAsync(request.SchoolId, cancellationToken);
        var assignedRoles = roles.Where(r => roleIds.Contains(r.Id)).ToList();

        if (assignedRoles.Count == 0)
        {
            return [];
        }

        var permissionIds = assignedRoles
            .SelectMany(r => r.Permissions.Select(rp => rp.PermissionId))
            .ToHashSet();

        if (permissionIds.Count == 0)
        {
            return [];
        }

        var allPermissions = await permissionRepository.GetAllAsync(cancellationToken);

        return allPermissions
            .Where(p => permissionIds.Contains(p.Id))
            .Select(p => p.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n)
            .ToArray();
    }
}
