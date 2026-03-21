using Edvantix.Constants.Permissions;

namespace Edvantix.Organizations.Features.GroupMembership.GetGroupMembers;

/// <summary>GET /v1/groups/{groupId}/members — lists all members of a group.</summary>
public sealed class GetGroupMembersEndpoint : IEndpoint<Ok<List<GroupMemberDto>>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups/{groupId:guid}/members",
                async (Guid groupId, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(groupId, sender, cancellationToken)
            )
            .Produces<List<GroupMemberDto>>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetGroupMembers")
            .WithTags("GroupMembership")
            .WithSummary("List group members")
            .WithDescription("Returns all students enrolled in the specified group.")
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(GroupsPermissions.ManageGroupMembership);
    }

    public async Task<Ok<List<GroupMemberDto>>> HandleAsync(
        Guid groupId,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetGroupMembersQuery(groupId), cancellationToken);

        return TypedResults.Ok(result);
    }
}
