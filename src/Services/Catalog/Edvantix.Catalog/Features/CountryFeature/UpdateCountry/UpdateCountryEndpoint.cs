namespace Edvantix.Catalog.Features.CountryFeature.UpdateCountry;

/// <summary>
/// Тело запроса для PUT /countries/{code}.
/// </summary>
public sealed record UpdateCountryRequest(
    string NameRu,
    string NameEn,
    string PhonePrefix,
    string CurrencyCode,
    bool? IsActive
);

/// <summary>
/// PUT /countries/{code} — обновление страны. Требует роли Admin.
/// HTTP 404 — если страна не найдена.
/// </summary>
public sealed class UpdateCountryEndpoint
    : IEndpoint<Ok<CountryModel>, UpdateCountryCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/countries/{code}",
                async (
                    string code,
                    UpdateCountryRequest body,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new UpdateCountryCommand(
                            code,
                            body.NameRu,
                            body.NameEn,
                            body.PhonePrefix,
                            body.CurrencyCode,
                            body.IsActive
                        ),
                        sender,
                        ct
                    )
            )
            .WithName("UpdateCountry")
            .WithTags("Admin.Countries")
            .WithSummary("Обновить страну")
            .WithDescription("Изменяет поля страны. Доступно только администраторам.")
            .Produces<CountryModel>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    /// <inheritdoc/>
    public async Task<Ok<CountryModel>> HandleAsync(
        UpdateCountryCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
