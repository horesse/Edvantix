namespace Edvantix.Catalog.Features.LanguageFeature.CreateLanguage;

/// <summary>
/// POST /languages — создание языка. Требует роли Admin.
/// HTTP 201 — успех, HTTP 400 — невалидные данные, HTTP 409 — код уже занят.
/// </summary>
public sealed class CreateLanguageEndpoint
    : IEndpoint<Created<LanguageModel>, CreateLanguageCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/languages",
                async (CreateLanguageCommand req, ISender sender, CancellationToken ct) =>
                    await HandleAsync(req, sender, ct)
            )
            .WithName("CreateLanguage")
            .WithTags("Admin.Languages")
            .WithSummary("Создать язык")
            .WithDescription("Добавляет новый язык в справочник. Доступно только администраторам.")
            .Produces<LanguageModel>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    /// <inheritdoc/>
    public async Task<Created<LanguageModel>> HandleAsync(
        CreateLanguageCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/languages/{result.Code}", result);
    }
}
