namespace Edvantix.Organizational.Features.Directories.Levels.Reorder;

/// <summary>PATCH /api/v1/directories/levels/reorder — переупорядочить уровни.</summary>
public sealed class ReorderLevelDirectoryEndpoint
    : IEndpoint<NoContent, ReorderLevelDirectoryCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/levels/reorder",
                async (
                    ReorderLevelDirectoryCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
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
        ReorderLevelDirectoryCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
