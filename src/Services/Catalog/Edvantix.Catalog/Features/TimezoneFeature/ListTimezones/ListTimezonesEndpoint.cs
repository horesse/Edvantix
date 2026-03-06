namespace Edvantix.Catalog.Features.TimezoneFeature.ListTimezones;

/// <summary>
/// GET /timezones — список часовых поясов с опциональным фильтром по активности.
/// Требует авторизации.
/// </summary>
public sealed class ListTimezonesEndpoint
    : IEndpoint<Ok<IReadOnlyList<TimezoneModel>>, ListTimezonesQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/timezones",
                async (
                    [AsParameters] ListTimezonesQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("ListTimezones")
            .WithTags("Timezones")
            .WithSummary("Список часовых поясов")
            .WithDescription(
                "Возвращает список часовых поясов IANA, отсортированных по смещению UTC. По умолчанию только активные."
            )
            .Produces<IReadOnlyList<TimezoneModel>>()
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<IReadOnlyList<TimezoneModel>>> HandleAsync(
        ListTimezonesQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
