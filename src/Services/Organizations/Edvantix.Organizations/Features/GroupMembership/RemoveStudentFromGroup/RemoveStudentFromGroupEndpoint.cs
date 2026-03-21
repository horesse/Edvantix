using Edvantix.Constants.Permissions;

namespace Edvantix.Organizations.Features.GroupMembership.RemoveStudentFromGroup;

/// <summary>DELETE /v1/groups/{groupId}/members/{profileId} — removes a student from a group.</summary>
public sealed class RemoveStudentFromGroupEndpoint : IEndpoint<NoContent, Guid, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/groups/{groupId:guid}/members/{profileId:guid}",
                async (
                    Guid groupId,
                    Guid profileId,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(groupId, profileId, sender, cancellationToken)
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("RemoveStudentFromGroup")
            .WithTags("GroupMembership")
            .WithSummary("Remove student from group")
            .WithDescription(
                "Removes a student from a group. No-op if the student is not a member."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(GroupsPermissions.ManageGroupMembership);
    }

    public async Task<NoContent> HandleAsync(
        Guid groupId,
        Guid profileId,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new RemoveStudentFromGroupCommand(groupId, profileId), cancellationToken);

        return TypedResults.NoContent();
    }
}
