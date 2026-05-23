namespace Edvantix.Organizational.Features.Directories.Rooms.Update;

/// <summary>Эндпоинт обновления кабинета.</summary>
public sealed class UpdateRoomEndpoint : IEndpoint<Ok<RoomDto>, UpdateRoomCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/directories/rooms/{id:guid}",
                async (
                    UpdateRoomCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("UpdateRoom")
            .WithTags("Кабинеты")
            .WithSummary("Обновить кабинет")
            .Produces<RoomDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<RoomDto>> HandleAsync(
        UpdateRoomCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);

        return TypedResults.Ok(dto);
    }
}
