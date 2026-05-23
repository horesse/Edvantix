namespace Edvantix.Organizational.Features.Directories.LeadSources.List;

/// <summary>Эндпоинт постраничного списка источников привлечения.</summary>
public sealed class ListLeadSourcesEndpoint
    : IEndpoint<Ok<PagedResult<LeadSourceListItemDto>>, ListLeadSourcesQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/sources",
                async (
                    [AsParameters] ListLeadSourcesQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("ListLeadSources")
            .WithTags("Источники привлечения")
            .WithSummary("Получить список источников привлечения организации")
            .ProducesGet<PagedResult<LeadSourceListItemDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<PagedResult<LeadSourceListItemDto>>> HandleAsync(
        ListLeadSourcesQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
