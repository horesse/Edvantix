namespace Edvantix.Catalog.Features.CountryFeature.CreateCountry;

/// <summary>
/// POST /countries — создание страны. Требует роли Admin.
/// HTTP 201 — успех, HTTP 400 — невалидные данные, HTTP 409 — код уже занят.
/// </summary>
public sealed class CreateCountryEndpoint
    : IEndpoint<Created<CountryModel>, CreateCountryCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/countries",
                async (CreateCountryCommand req, ISender sender, CancellationToken ct) =>
                    await HandleAsync(req, sender, ct)
            )
            .WithName("CreateCountry")
            .WithTags("Admin.Countries")
            .WithSummary("Создать страну")
            .WithDescription(
                "Добавляет новую страну в справочник. Доступно только администраторам."
            )
            .Produces<CountryModel>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    /// <inheritdoc/>
    public async Task<Created<CountryModel>> HandleAsync(
        CreateCountryCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/countries/{result.Code}", result);
    }
}
