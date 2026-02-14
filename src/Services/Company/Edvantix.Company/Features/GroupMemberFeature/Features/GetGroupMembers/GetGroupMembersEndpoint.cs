using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Features.GroupMemberFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.GroupMemberFeature.Features.GetGroupMembers;

public class GetGroupMembersEndpoint
    : IEndpoint<Ok<IEnumerable<GroupMemberModel>>, GetGroupMembersQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups/{groupId:long}/members",
                async (long groupId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetGroupMembersQuery(groupId), sender, ct)
            )
            .WithName("GetGroupMembers")
            .WithTags("Group Members")
            .WithSummary("Список участников группы")
            .WithDescription(
                "Возвращает список участников группы. Доступно участникам организации."
            )
            .Produces<IEnumerable<GroupMemberModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<Ok<IEnumerable<GroupMemberModel>>> HandleAsync(
        GetGroupMembersQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
