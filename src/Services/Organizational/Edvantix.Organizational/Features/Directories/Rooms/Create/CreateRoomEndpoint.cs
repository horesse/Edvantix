namespace Edvantix.Organizational.Features.Directories.Rooms.Create;

/// <summary>Эндпоинт создания кабинета.</summary>
public sealed class CreateRoomEndpoint : IEndpoint<Created<RoomDto>, CreateRoomCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/rooms",
                async (
                    CreateRoomCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("CreateRoom")
            .WithTags("Кабинеты")
            .WithSummary("Создать кабинет")
            .ProducesPost<RoomDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Created<RoomDto>> HandleAsync(
        CreateRoomCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);

        return TypedResults.Created($"/api/v1/directories/rooms/{dto.Id}", dto);
    }
}
