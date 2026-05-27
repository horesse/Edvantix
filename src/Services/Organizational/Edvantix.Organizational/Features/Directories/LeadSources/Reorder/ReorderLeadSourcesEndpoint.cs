namespace Edvantix.Organizational.Features.Directories.LeadSources.Reorder;

/// <summary>PATCH /api/v1/directories/sources/reorder — переупорядочить источники привлечения.</summary>
public sealed class ReorderLeadSourcesEndpoint
    : IEndpoint<NoContent, ReorderLeadSourcesCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/sources/reorder",
                async (
                    ReorderLeadSourcesCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("ReorderLeadSources")
            .WithTags("Источники привлечения")
            .WithSummary("Изменить порядок источников привлечения")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ReorderLeadSourcesCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
