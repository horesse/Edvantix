namespace Edvantix.Organizational.Features.Groups.Members.List;

public sealed class GetGroupMembersEndpoint
    : IEndpoint<Ok<PagedResult<GroupMemberDto>>, GetGroupMembersQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups/{groupId:guid}/members",
                async (
                    [AsParameters] GetGroupMembersQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("GetGroupMembers")
            .WithTags("Участники группы")
            .WithSummary("Получить постраничный список участников группы")
            .WithPaginationHeaders()
            .Produces<PagedResult<GroupMemberDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<GroupMemberDto>>> HandleAsync(
        GetGroupMembersQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(result);
    }
}
