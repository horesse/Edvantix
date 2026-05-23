namespace Edvantix.Organizational.Features.Directories.LeadSources.Restore;

/// <summary>Эндпоинт восстановления источника привлечения из архива.</summary>
public sealed class RestoreLeadSourceEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/sources/{id:guid}/restore",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("RestoreLeadSource")
            .WithTags("Источники привлечения")
            .WithSummary("Восстановить источник привлечения из архива")
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
        await sender.Send(new RestoreLeadSourceCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
