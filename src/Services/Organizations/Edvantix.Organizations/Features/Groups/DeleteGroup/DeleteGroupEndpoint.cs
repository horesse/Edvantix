using Edvantix.Constants.Permissions;

namespace Edvantix.Organizations.Features.Groups.DeleteGroup;

/// <summary>DELETE /v1/groups/{id} — soft-deletes a group.</summary>
public sealed class DeleteGroupEndpoint : IEndpoint<NoContent, DeleteGroupCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/groups/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new DeleteGroupCommand { Id = id }, sender, cancellationToken)
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteGroup")
            .WithTags("Groups")
            .WithSummary("Delete a group")
            .WithDescription("Soft-deletes a group. The group must belong to the current tenant.")
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(GroupsPermissions.DeleteGroup);
    }

    public async Task<NoContent> HandleAsync(
        DeleteGroupCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
