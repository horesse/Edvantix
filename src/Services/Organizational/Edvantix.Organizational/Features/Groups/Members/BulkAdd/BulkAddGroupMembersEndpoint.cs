namespace Edvantix.Organizational.Features.Groups.Members.BulkAdd;

public sealed class BulkAddGroupMembersEndpoint
    : IEndpoint<IResult, BulkAddGroupMembersCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/groups/{groupId:guid}/members/bulk",
                async (
                    Guid groupId,
                    BulkAddGroupMembersCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(command with { GroupId = groupId }, sender, cancellationToken)
            )
            .WithName("BulkAddGroupMembers")
            .WithTags("Участники группы")
            .WithSummary("Пакетно добавить участников в группу")
            .WithDescription(
                "Возвращает 207 при частичном успехе (часть участников не удалось добавить)."
            )
            .Produces<BulkAddResult>(StatusCodes.Status200OK)
            .Produces<BulkAddResult>(StatusCodes.Status207MultiStatus)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(
        BulkAddGroupMembersCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);

        return result.Failed.Count > 0 && result.Added.Count > 0
            ? Results.Json(result, statusCode: StatusCodes.Status207MultiStatus)
            : TypedResults.Ok(result);
    }
}
