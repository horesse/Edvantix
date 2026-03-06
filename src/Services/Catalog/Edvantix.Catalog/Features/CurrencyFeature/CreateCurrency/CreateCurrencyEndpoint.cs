namespace Edvantix.Catalog.Features.CurrencyFeature.CreateCurrency;

/// <summary>
/// POST /currencies — создание валюты. Требует роли Admin.
/// HTTP 201 — успех, HTTP 400 — невалидные данные, HTTP 409 — код уже занят.
/// </summary>
public sealed class CreateCurrencyEndpoint
    : IEndpoint<Created<CurrencyModel>, CreateCurrencyCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/currencies",
                async (CreateCurrencyCommand req, ISender sender, CancellationToken ct) =>
                    await HandleAsync(req, sender, ct)
            )
            .WithName("CreateCurrency")
            .WithTags("Admin.Currencies")
            .WithSummary("Создать валюту")
            .WithDescription(
                "Добавляет новую валюту в справочник. Доступно только администраторам."
            )
            .Produces<CurrencyModel>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    /// <inheritdoc/>
    public async Task<Created<CurrencyModel>> HandleAsync(
        CreateCurrencyCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/currencies/{result.Code}", result);
    }
}
