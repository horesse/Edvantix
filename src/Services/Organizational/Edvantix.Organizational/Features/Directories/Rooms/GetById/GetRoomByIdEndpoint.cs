namespace Edvantix.Organizational.Features.Directories.Rooms.GetById;

/// <summary>Эндпоинт получения кабинета по идентификатору.</summary>
public sealed class GetRoomByIdEndpoint : IEndpoint<Results<Ok<RoomDto>, NotFound>, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/rooms/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetRoomById")
            .WithTags("Кабинеты")
            .WithSummary("Получить кабинет по идентификатору")
            .ProducesGet<RoomDto>(hasNotFound: true)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Results<Ok<RoomDto>, NotFound>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var dto = await sender.Send(new GetRoomByIdQuery(id), cancellationToken);

            return TypedResults.Ok(dto);
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}
