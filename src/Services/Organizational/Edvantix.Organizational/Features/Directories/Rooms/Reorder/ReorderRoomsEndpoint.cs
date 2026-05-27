using Edvantix.Organizational.Features.Directories;

namespace Edvantix.Organizational.Features.Directories.Rooms.Reorder;

/// <summary>PATCH /api/v1/directories/rooms/reorder — переупорядочить кабинеты.</summary>
public sealed class ReorderRoomsEndpoint : IEndpoint<NoContent, ReorderRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/rooms/reorder",
                async (ReorderRequest request, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("ReorderRooms")
            .WithTags("Кабинеты")
            .WithSummary("Изменить порядок кабинетов")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ReorderRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new ReorderRoomsCommand(request.OrderedIds), cancellationToken);

        return TypedResults.NoContent();
    }
}
