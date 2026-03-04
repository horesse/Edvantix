namespace Edvantix.Catalog.Features.LanguageFeature.ListLanguages;

/// <summary>
/// GET /languages — список языков с опциональным фильтром по активности.
/// Требует авторизации.
/// </summary>
public sealed class ListLanguagesEndpoint
    : IEndpoint<Ok<IReadOnlyList<LanguageModel>>, ListLanguagesQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/languages",
                async (
                    [AsParameters] ListLanguagesQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("ListLanguages")
            .WithTags("Languages")
            .WithSummary("Список языков")
            .WithDescription("Возвращает список языков ISO 639-1. По умолчанию только активные.")
            .Produces<IReadOnlyList<LanguageModel>>()
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<IReadOnlyList<LanguageModel>>> HandleAsync(
        ListLanguagesQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
