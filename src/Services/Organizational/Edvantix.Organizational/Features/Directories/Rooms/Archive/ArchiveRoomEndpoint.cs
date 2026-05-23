namespace Edvantix.Organizational.Features.Directories.Rooms.Archive;

/// <summary>Эндпоинт архивации кабинета.</summary>
public sealed class ArchiveRoomEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/rooms/{id:guid}/archive",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("ArchiveRoom")
            .WithTags("Кабинеты")
            .WithSummary("Архивировать кабинет")
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
        await sender.Send(new ArchiveRoomCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
