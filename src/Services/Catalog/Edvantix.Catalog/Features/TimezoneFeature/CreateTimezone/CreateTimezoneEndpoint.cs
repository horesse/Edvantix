namespace Edvantix.Catalog.Features.TimezoneFeature.CreateTimezone;

/// <summary>
/// POST /timezones — создание часового пояса. Требует роли Admin.
/// HTTP 201 — успех, HTTP 400 — невалидные данные, HTTP 409 — код уже занят.
/// </summary>
public sealed class CreateTimezoneEndpoint
    : IEndpoint<Created<TimezoneModel>, CreateTimezoneCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/timezones",
                async (CreateTimezoneCommand req, ISender sender, CancellationToken ct) =>
                    await HandleAsync(req, sender, ct)
            )
            .WithName("CreateTimezone")
            .WithTags("Admin.Timezones")
            .WithSummary("Создать часовой пояс")
            .WithDescription(
                "Добавляет новый часовой пояс в справочник. Доступно только администраторам."
            )
            .Produces<TimezoneModel>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    /// <inheritdoc/>
    public async Task<Created<TimezoneModel>> HandleAsync(
        CreateTimezoneCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        var encodedCode = Uri.EscapeDataString(result.Code);
        return TypedResults.Created($"/api/v1/timezones/{encodedCode}", result);
    }
}
