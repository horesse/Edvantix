namespace Edvantix.Catalog.Features.CurrencyFeature.ListCurrencies;

/// <summary>
/// GET /currencies — список валют с опциональным фильтром по активности.
/// Требует авторизации (любой авторизованный пользователь).
/// </summary>
public sealed class ListCurrenciesEndpoint
    : IEndpoint<Ok<IReadOnlyList<CurrencyModel>>, ListCurrenciesQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/currencies",
                async (
                    [AsParameters] ListCurrenciesQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("ListCurrencies")
            .WithTags("Currencies")
            .WithSummary("Список валют")
            .WithDescription("Возвращает список валют ISO 4217. По умолчанию только активные.")
            .Produces<IReadOnlyList<CurrencyModel>>()
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<IReadOnlyList<CurrencyModel>>> HandleAsync(
        ListCurrenciesQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
