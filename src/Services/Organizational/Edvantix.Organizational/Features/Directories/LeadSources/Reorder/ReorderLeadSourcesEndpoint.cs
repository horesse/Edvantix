using Edvantix.Organizational.Features.Directories;

namespace Edvantix.Organizational.Features.Directories.LeadSources.Reorder;

/// <summary>PATCH /api/v1/directories/sources/reorder — переупорядочить источники привлечения.</summary>
public sealed class ReorderLeadSourcesEndpoint : IEndpoint<NoContent, ReorderRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/sources/reorder",
                async (
                    ReorderRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
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
        ReorderRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new ReorderLeadSourcesCommand(request.OrderedIds), cancellationToken);

        return TypedResults.NoContent();
    }
}
