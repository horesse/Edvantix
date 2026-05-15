namespace Edvantix.Organizational.Features.Groups.Members.Remove;

public sealed class RemoveGroupMemberEndpoint
    : IEndpoint<NoContent, RemoveGroupMemberCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/groups/{groupId:guid}/members/{memberId:guid}",
                async (
                    Guid groupId,
                    Guid memberId,
                    RemoveGroupMemberCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        command with { GroupId = groupId, MemberId = memberId },
                        sender,
                        cancellationToken
                    )
            )
            .WithName("RemoveGroupMember")
            .WithTags("Участники группы")
            .WithSummary("Убрать участника из группы")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
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
