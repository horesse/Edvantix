namespace Edvantix.Organizational.Features.Rooms.Create;

public sealed class CreateRoomEndpoint
    : IEndpoint<Created<Guid>, CreateRoomCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/rooms",
                async (
                    CreateRoomCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .WithName("CreateRoom")
            .WithTags("Кабинеты")
            .WithSummary("Создать кабинет в организации")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateRoomCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location = linker.GetPathByName("GetAvailableRooms") ?? "/api/rooms";

        return TypedResults.Created(location, id);
    }
}
