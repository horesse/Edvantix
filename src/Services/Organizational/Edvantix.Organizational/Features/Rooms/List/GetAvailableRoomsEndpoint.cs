namespace Edvantix.Organizational.Features.Rooms.List;

public sealed class GetAvailableRoomsEndpoint
    : IEndpoint<Ok<IReadOnlyList<RoomDto>>, GetAvailableRoomsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/rooms",
                async (
                    [AsParameters] GetAvailableRoomsQuery request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("GetAvailableRooms")
            .WithTags("Кабинеты")
            .WithSummary("Получить список доступных кабинетов организации")
            .Produces<IReadOnlyList<RoomDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyList<RoomDto>>> HandleAsync(
        GetAvailableRoomsQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);

        return TypedResults.Ok(result);
    }
}
