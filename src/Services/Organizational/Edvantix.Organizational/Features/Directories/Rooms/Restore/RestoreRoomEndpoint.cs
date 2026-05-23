namespace Edvantix.Organizational.Features.Directories.Rooms.Restore;

/// <summary>Эндпоинт восстановления кабинета из архива.</summary>
public sealed class RestoreRoomEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/rooms/{id:guid}/restore",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("RestoreRoom")
            .WithTags("Кабинеты")
            .WithSummary("Восстановить кабинет из архива")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<NoContent> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new RestoreRoomCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
