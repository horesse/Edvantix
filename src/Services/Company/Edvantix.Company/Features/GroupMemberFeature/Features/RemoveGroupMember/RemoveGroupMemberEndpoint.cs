using Edvantix.Chassis.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.GroupMemberFeature.Features.RemoveGroupMember;

public class RemoveGroupMemberEndpoint
    : IEndpoint<NoContent, RemoveGroupMemberCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/groups/{groupId:long}/members/{memberId:guid}",
                async (long groupId, Guid memberId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new RemoveGroupMemberCommand(groupId, memberId), sender, ct)
            )
            .WithName("RemoveGroupMember")
            .WithTags("Group Members")
            .WithSummary("Удалить участника из группы")
            .WithDescription("Удаляет участника из группы (мягкое удаление). Доступно владельцу, менеджеру и учителю/менеджеру группы.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        RemoveGroupMemberCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
