using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.GroupMemberFeature.Features.AddGroupMember;

public sealed record AddGroupMemberRequest(int ProfileId, GroupRole Role);

public class AddGroupMemberEndpoint : IEndpoint<Created<Guid>, AddGroupMemberCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/groups/{groupId:long}/members",
                async (
                    long groupId,
                    AddGroupMemberRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new AddGroupMemberCommand(
                        groupId,
                        request.ProfileId,
                        request.Role
                    );
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("AddGroupMember")
            .WithTags("Group Members")
            .WithSummary("Добавить участника в группу")
            .WithDescription(
                "Добавляет участника организации в группу. Доступно владельцу, менеджеру и учителю/менеджеру группы."
            )
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        AddGroupMemberCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/groups/{command.GroupId}/members/{id}", id);
    }
}
