namespace Edvantix.Organizations.Features.Roles.DeleteRole;

/// <summary>DELETE /v1/roles/{id} — soft-deletes a role. Returns 409 if the role has active user assignments.</summary>
public sealed class DeleteRoleEndpoint : IEndpoint<NoContent, DeleteRoleCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/roles/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new DeleteRoleCommand { Id = id }, sender, cancellationToken)
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .WithName("DeleteRole")
            .WithTags("Roles")
            .WithSummary("Delete a role")
            .WithDescription(
                "Soft-deletes a role. Returns 409 Conflict if the role is currently assigned to any users."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        DeleteRoleCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
