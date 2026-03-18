namespace Edvantix.Organizations.Features.UserRoleAssignments.RevokeRole;

/// <summary>
/// DELETE /v1/user-role-assignments/{profileId:guid}/{roleId:guid} — revokes a role from a user.
/// Hard-deletes the assignment; returns 404 if the assignment does not exist.
/// </summary>
public sealed class RevokeRoleEndpoint : IEndpoint<NoContent, Guid, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/user-role-assignments/{profileId:guid}/{roleId:guid}",
                async (
                    Guid profileId,
                    Guid roleId,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        profileId,
                        roleId,
                        sender,
                        cancellationToken
                    )
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("RevokeRole")
            .WithTags("UserRoleAssignments")
            .WithSummary("Revoke a role from a user")
            .WithDescription(
                "Hard-deletes the role assignment for the specified profile and role within the current tenant."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        Guid profileId,
        Guid roleId,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new RevokeRoleCommand { ProfileId = profileId, RoleId = roleId };
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
