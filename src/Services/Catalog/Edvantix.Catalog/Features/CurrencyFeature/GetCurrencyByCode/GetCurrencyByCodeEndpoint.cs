namespace Edvantix.Catalog.Features.CurrencyFeature.GetCurrencyByCode;

/// <summary>
/// GET /currencies/{code} — валюта по ISO 4217 коду.
/// Возвращает 404, если валюта не найдена.
/// Требует авторизации.
/// </summary>
public sealed class GetCurrencyByCodeEndpoint
    : IEndpoint<Ok<CurrencyModel>, GetCurrencyByCodeQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/currencies/{code}",
                async (string code, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetCurrencyByCodeQuery(code), sender, ct)
            )
            .WithName("GetCurrencyByCode")
            .WithTags("Currencies")
            .WithSummary("Валюта по коду")
            .WithDescription("Возвращает валюту по алфавитному коду ISO 4217 (например, \"USD\").")
            .Produces<CurrencyModel>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<CurrencyModel>> HandleAsync(
        GetCurrencyByCodeQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
