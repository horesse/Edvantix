namespace Edvantix.Catalog.Features.RegionFeature.ListRegions;

/// <summary>
/// GET /regions — список регионов с опциональным фильтром по активности.
/// Требует авторизации.
/// </summary>
public sealed class ListRegionsEndpoint
    : IEndpoint<Ok<IReadOnlyList<RegionModel>>, ListRegionsQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/regions",
                async (
                    [AsParameters] ListRegionsQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("ListRegions")
            .WithTags("Regions")
            .WithSummary("Список регионов")
            .WithDescription(
                "Возвращает список географических регионов для сегментации рынков. По умолчанию только активные."
            )
            .Produces<IReadOnlyList<RegionModel>>()
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<IReadOnlyList<RegionModel>>> HandleAsync(
        ListRegionsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
