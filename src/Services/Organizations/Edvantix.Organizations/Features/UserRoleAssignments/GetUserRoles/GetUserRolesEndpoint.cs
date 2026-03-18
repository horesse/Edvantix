namespace Edvantix.Organizations.Features.UserRoleAssignments.GetUserRoles;

/// <summary>
/// GET /v1/user-role-assignments/{profileId:guid} — returns all roles for a user in the current tenant.
/// Returns an empty list if the profile has no assignments.
/// </summary>
public sealed class GetUserRolesEndpoint : IEndpoint<Ok<List<UserRoleItem>>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/user-role-assignments/{profileId:guid}",
                async (Guid profileId, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(profileId, sender, cancellationToken)
            )
            .Produces<List<UserRoleItem>>(StatusCodes.Status200OK)
            .WithName("GetUserRoles")
            .WithTags("UserRoleAssignments")
            .WithSummary("Get roles for a user")
            .WithDescription(
                "Returns all roles assigned to the specified profile within the current tenant, ordered by role name."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<List<UserRoleItem>>> HandleAsync(
        Guid profileId,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetUserRolesQuery { ProfileId = profileId };
        var roles = await sender.Send(query, cancellationToken);

        return TypedResults.Ok(roles);
    }
}
