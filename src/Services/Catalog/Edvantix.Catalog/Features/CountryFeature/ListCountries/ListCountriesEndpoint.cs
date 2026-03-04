namespace Edvantix.Catalog.Features.CountryFeature.ListCountries;

/// <summary>
/// GET /countries — список стран с опциональным фильтром по активности.
/// Требует авторизации.
/// </summary>
public sealed class ListCountriesEndpoint
    : IEndpoint<Ok<IReadOnlyList<CountryModel>>, ListCountriesQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/countries",
                async (
                    [AsParameters] ListCountriesQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("ListCountries")
            .WithTags("Countries")
            .WithSummary("Список стран")
            .WithDescription("Возвращает список стран ISO 3166-1. По умолчанию только активные.")
            .Produces<IReadOnlyList<CountryModel>>()
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<IReadOnlyList<CountryModel>>> HandleAsync(
        ListCountriesQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
