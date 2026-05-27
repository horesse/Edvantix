namespace Edvantix.Organizational.Features.Directories.Rooms.List;

/// <summary>Эндпоинт постраничного списка кабинетов.</summary>
public sealed class ListRoomsEndpoint
    : IEndpoint<Ok<PagedResult<RoomListItemDto>>, ListRoomsQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/rooms",
                async (
                    [AsParameters] ListRoomsQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("ListRooms")
            .WithTags("Кабинеты")
            .WithSummary("Получить список кабинетов организации")
            .WithPaginationHeaders()
            .ProducesGet<PagedResult<RoomListItemDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<PagedResult<RoomListItemDto>>> HandleAsync(
        ListRoomsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
