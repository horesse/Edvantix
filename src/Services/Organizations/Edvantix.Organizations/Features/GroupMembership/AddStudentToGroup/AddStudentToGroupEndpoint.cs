using Edvantix.Constants.Permissions;

namespace Edvantix.Organizations.Features.GroupMembership.AddStudentToGroup;

/// <summary>POST /v1/groups/{groupId}/members — adds a student to a group.</summary>
public sealed class AddStudentToGroupEndpoint
    : IEndpoint<NoContent, Guid, AddStudentToGroupBody, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/groups/{groupId:guid}/members",
                async (
                    Guid groupId,
                    AddStudentToGroupBody body,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(groupId, body, sender, cancellationToken)
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithName("AddStudentToGroup")
            .WithTags("GroupMembership")
            .WithSummary("Add student to group")
            .WithDescription(
                "Adds a student (by Persona profile) to a group. Idempotent — no error if already a member."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(GroupsPermissions.ManageGroupMembership);
    }

    public async Task<NoContent> HandleAsync(
        Guid groupId,
        AddStudentToGroupBody body,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new AddStudentToGroupCommand(groupId, body.ProfileId), cancellationToken);

        return TypedResults.NoContent();
    }
}

/// <summary>Request body for <see cref="AddStudentToGroupEndpoint"/>.</summary>
/// <param name="ProfileId">The Persona profile identifier of the student to add.</param>
public sealed record AddStudentToGroupBody(Guid ProfileId);
