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
                    BulkAddGroupMembersRequest body,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new BulkAddGroupMembersCommand(groupId, body.Items),
                        sender,
                        cancellationToken
                    )
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

/// <summary>Тело запроса для пакетного добавления участников в группу.</summary>
public sealed record BulkAddGroupMembersRequest(
    [property: Description("Список участников для добавления")] IReadOnlyList<BulkAddItem> Items
);
