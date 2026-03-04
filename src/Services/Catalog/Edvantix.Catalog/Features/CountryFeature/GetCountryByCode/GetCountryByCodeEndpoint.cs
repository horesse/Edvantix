namespace Edvantix.Catalog.Features.CountryFeature.GetCountryByCode;

/// <summary>
/// GET /countries/{code} — страна по ISO 3166-1 alpha-2 коду.
/// Возвращает 404, если страна не найдена.
/// Требует авторизации.
/// </summary>
public sealed class GetCountryByCodeEndpoint
    : IEndpoint<Ok<CountryModel>, GetCountryByCodeQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/countries/{code}",
                async (string code, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetCountryByCodeQuery(code), sender, ct)
            )
            .WithName("GetCountryByCode")
            .WithTags("Countries")
            .WithSummary("Страна по коду")
            .WithDescription(
                "Возвращает страну по двухбуквенному коду ISO 3166-1 alpha-2 (например, \"RU\")."
            )
            .Produces<CountryModel>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<CountryModel>> HandleAsync(
        GetCountryByCodeQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
