using Edvantix.Constants.Permissions;

namespace Edvantix.Organizations.Features.Groups.GetGroups;

/// <summary>GET /v1/groups — returns all groups for the current tenant.</summary>
public sealed class GetGroupsEndpoint : IEndpoint<Ok<List<GroupDto>>, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(sender, cancellationToken)
            )
            .Produces<List<GroupDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("GetGroups")
            .WithTags("Groups")
            .WithSummary("List groups")
            .WithDescription(
                "Returns all groups for the current tenant. Does not include membership data."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(GroupsPermissions.CreateGroup);
    }

    public async Task<Ok<List<GroupDto>>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetGroupsQuery(), cancellationToken);

        return TypedResults.Ok(result);
    }
}
