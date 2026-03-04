namespace Edvantix.Catalog.Features.CurrencyFeature.UpdateCurrency;

/// <summary>
/// Тело запроса для PUT /currencies/{code}.
/// </summary>
public sealed record UpdateCurrencyRequest(
    string NameRu,
    string NameEn,
    string Symbol,
    bool? IsActive
);

/// <summary>
/// PUT /currencies/{code} — обновление валюты. Требует роли Admin.
/// HTTP 404 — если валюта не найдена.
/// </summary>
public sealed class UpdateCurrencyEndpoint
    : IEndpoint<Ok<CurrencyModel>, UpdateCurrencyCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/currencies/{code}",
                async (
                    string code,
                    UpdateCurrencyRequest body,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new UpdateCurrencyCommand(
                            code,
                            body.NameRu,
                            body.NameEn,
                            body.Symbol,
                            body.IsActive
                        ),
                        sender,
                        ct
                    )
            )
            .WithName("UpdateCurrency")
            .WithTags("Admin.Currencies")
            .WithSummary("Обновить валюту")
            .WithDescription("Изменяет поля валюты. Доступно только администраторам.")
            .Produces<CurrencyModel>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    /// <inheritdoc/>
    public async Task<Ok<CurrencyModel>> HandleAsync(
        UpdateCurrencyCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
