namespace Edvantix.Catalog.Features.TimezoneFeature.UpdateTimezone;

/// <summary>
/// Тело запроса для PUT /timezones/{**code}.
/// </summary>
public sealed record UpdateTimezoneRequest(
    string NameRu,
    string NameEn,
    string DisplayName,
    int UtcOffsetMinutes,
    bool? IsActive
);

/// <summary>
/// PUT /timezones/{**code} — обновление часового пояса. Требует роли Admin.
/// Маршрут использует catch-all параметр <c>{**code}</c>, так как IANA-коды содержат символ «/»
/// (например, «Europe/Moscow»).
/// HTTP 404 — если часовой пояс не найден.
/// </summary>
public sealed class UpdateTimezoneEndpoint
    : IEndpoint<Ok<TimezoneModel>, UpdateTimezoneCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/timezones/{**code}",
                async (
                    string code,
                    UpdateTimezoneRequest body,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new UpdateTimezoneCommand(
                            code,
                            body.NameRu,
                            body.NameEn,
                            body.DisplayName,
                            body.UtcOffsetMinutes,
                            body.IsActive
                        ),
                        sender,
                        ct
                    )
            )
            .WithName("UpdateTimezone")
            .WithTags("Admin.Timezones")
            .WithSummary("Обновить часовой пояс")
            .WithDescription("Изменяет поля часового пояса. Доступно только администраторам.")
            .Produces<TimezoneModel>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    /// <inheritdoc/>
    public async Task<Ok<TimezoneModel>> HandleAsync(
        UpdateTimezoneCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
