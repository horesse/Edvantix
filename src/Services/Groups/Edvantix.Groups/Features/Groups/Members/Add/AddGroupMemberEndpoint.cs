namespace Edvantix.Groups.Features.Groups.Members.Add;

public sealed class AddGroupMemberEndpoint
    : IEndpoint<Created<Guid>, AddGroupMemberCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/groups/{groupId:guid}/members",
                async (
                    Guid groupId,
                    AddGroupMemberCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        command with
                        {
                            GroupId = groupId,
                        },
                        sender,
                        linker,
                        cancellationToken
                    )
            )
            .WithName("AddGroupMember")
            .WithTags("Участники группы")
            .WithSummary("Добавить участника в группу")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        AddGroupMemberCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location =
            linker.GetPathByName("GetGroupMembers", new { groupId = command.GroupId })
            ?? $"/api/groups/{command.GroupId}/members";

        return TypedResults.Created(location, id);
    }
}
