using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.GroupMemberFeature.Features.UpdateGroupMemberRole;

public sealed record UpdateGroupMemberRoleRequest(GroupRole NewRole);

public class UpdateGroupMemberRoleEndpoint
    : IEndpoint<NoContent, UpdateGroupMemberRoleCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/groups/{groupId:long}/members/{memberId:guid}/role",
                async (long groupId, Guid memberId, UpdateGroupMemberRoleRequest request, ISender sender, CancellationToken ct) =>
                {
                    var command = new UpdateGroupMemberRoleCommand(groupId, memberId, request.NewRole);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdateGroupMemberRole")
            .WithTags("Group Members")
            .WithSummary("Изменить роль участника группы")
            .WithDescription("Изменяет роль участника группы. Доступно владельцу, менеджеру и учителю/менеджеру группы.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateGroupMemberRoleCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
