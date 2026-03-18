namespace Edvantix.Organizations.Features.Roles.AssignPermissions;

/// <summary>PUT /v1/roles/{id}/permissions — assigns a set of permissions to a role.</summary>
public sealed class AssignPermissionsEndpoint
    : IEndpoint<NoContent, AssignPermissionsCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/roles/{id:guid}/permissions",
                async (
                    Guid id,
                    AssignPermissionsRequest body,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new AssignPermissionsCommand
                        {
                            RoleId = id,
                            PermissionNames = body.PermissionNames,
                        },
                        sender,
                        cancellationToken
                    )
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("AssignPermissions")
            .WithTags("Roles")
            .WithSummary("Assign permissions to a role")
            .WithDescription(
                "Replaces the permission set of a role. All names must exist in the permission catalogue. "
                    + "Pass an empty list to clear all permissions."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        AssignPermissionsCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}

/// <summary>Request body for the assign-permissions endpoint.</summary>
public sealed record AssignPermissionsRequest(List<string> PermissionNames);
