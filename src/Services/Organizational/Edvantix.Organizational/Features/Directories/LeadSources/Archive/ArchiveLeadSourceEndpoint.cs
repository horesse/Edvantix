namespace Edvantix.Organizational.Features.Directories.LeadSources.Archive;

/// <summary>Эндпоинт архивации источника привлечения.</summary>
public sealed class ArchiveLeadSourceEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/sources/{id:guid}/archive",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("ArchiveLeadSource")
            .WithTags("Источники привлечения")
            .WithSummary("Архивировать источник привлечения")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<NoContent> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new ArchiveLeadSourceCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
