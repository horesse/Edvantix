using Edvantix.Organizational.Features.GroupMemberFeature.Models;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Organizational.Features.GroupMemberFeature.Features.GetGroupMembers;

public sealed class GetGroupMembersEndpoint
    : IEndpoint<Ok<PagedResult<GroupMemberModel>>, GetGroupMembersQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups/members",
                async (
                    [AsParameters] GetGroupMembersQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("GetGroupMembers")
            .WithTags("Group Members")
            .WithSummary("Список участников группы")
            .WithDescription(
                "Возвращает пагинированный список участников группы. Доступно участникам организации."
            )
            .WithPaginationHeaders()
            .Produces<PagedResult<GroupMemberModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<GroupMemberModel>>> HandleAsync(
        GetGroupMembersQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
