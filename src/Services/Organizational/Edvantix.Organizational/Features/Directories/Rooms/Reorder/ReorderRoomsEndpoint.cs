namespace Edvantix.Organizational.Features.Directories.Rooms.Reorder;

/// <summary>PATCH /api/v1/directories/rooms/reorder — переупорядочить кабинеты.</summary>
public sealed class ReorderRoomsEndpoint : IEndpoint<NoContent, ReorderRoomsCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/rooms/reorder",
                async (
                    ReorderRoomsCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("ReorderRooms")
            .WithTags("Справочник: Кабинеты")
            .WithSummary("Изменить порядок кабинетов")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ReorderRoomsCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
