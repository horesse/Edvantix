using Edvantix.Organizational.Features.Directories;

namespace Edvantix.Organizational.Features.Directories.Levels.Reorder;

/// <summary>PATCH /api/v1/directories/levels/reorder — переупорядочить уровни.</summary>
public sealed class ReorderLevelDirectoryEndpoint : IEndpoint<NoContent, ReorderRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/levels/reorder",
                async (ReorderRequest request, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("ReorderLevelDirectory")
            .WithTags("Справочник: Уровни")
            .WithSummary("Изменить порядок уровней")
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
        await sender.Send(new ReorderLevelDirectoryCommand(request.OrderedIds), cancellationToken);

        return TypedResults.NoContent();
    }
}
