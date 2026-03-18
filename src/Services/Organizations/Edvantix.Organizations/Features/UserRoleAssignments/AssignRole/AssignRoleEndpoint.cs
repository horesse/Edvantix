namespace Edvantix.Organizations.Features.UserRoleAssignments.AssignRole;

/// <summary>
/// POST /v1/user-role-assignments — assigns a role to a user profile in the current tenant.
/// Validates both role existence and profile existence (via Persona gRPC) before creating the assignment.
/// </summary>
public sealed class AssignRoleEndpoint
    : IEndpoint<Created<Guid>, AssignRoleCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/user-role-assignments",
                async (
                    AssignRoleCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("AssignRole")
            .WithTags("UserRoleAssignments")
            .WithSummary("Assign a role to a user")
            .WithDescription(
                "Assigns a role to a user profile within the current tenant. "
                    + "Validates role existence and profile existence via Persona gRPC before creating the assignment."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        AssignRoleCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location =
            linker.GetPathByName("GetUserRoles", new { profileId = command.ProfileId })
            ?? $"/api/v1/user-role-assignments/{command.ProfileId}";

        return TypedResults.Created(location, id);
    }
}
