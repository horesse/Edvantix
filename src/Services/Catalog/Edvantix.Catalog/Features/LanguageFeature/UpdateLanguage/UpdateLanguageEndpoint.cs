namespace Edvantix.Catalog.Features.LanguageFeature.UpdateLanguage;

/// <summary>
/// Тело запроса для PUT /languages/{code}.
/// </summary>
public sealed record UpdateLanguageRequest(
    string NameRu,
    string NameEn,
    string NativeName,
    bool? IsActive
);

/// <summary>
/// PUT /languages/{code} — обновление языка. Требует роли Admin.
/// HTTP 404 — если язык не найден.
/// </summary>
public sealed class UpdateLanguageEndpoint
    : IEndpoint<Ok<LanguageModel>, UpdateLanguageCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/languages/{code}",
                async (
                    string code,
                    UpdateLanguageRequest body,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new UpdateLanguageCommand(
                            code,
                            body.NameRu,
                            body.NameEn,
                            body.NativeName,
                            body.IsActive
                        ),
                        sender,
                        ct
                    )
            )
            .WithName("UpdateLanguage")
            .WithTags("Admin.Languages")
            .WithSummary("Обновить язык")
            .WithDescription("Изменяет поля языка. Доступно только администраторам.")
            .Produces<LanguageModel>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    /// <inheritdoc/>
    public async Task<Ok<LanguageModel>> HandleAsync(
        UpdateLanguageCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
