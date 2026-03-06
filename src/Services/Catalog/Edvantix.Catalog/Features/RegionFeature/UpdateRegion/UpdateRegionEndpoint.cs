namespace Edvantix.Catalog.Features.RegionFeature.UpdateRegion;

/// <summary>
/// Тело запроса для PUT /regions/{code}.
/// </summary>
public sealed record UpdateRegionRequest(string NameRu, string NameEn, bool? IsActive);

/// <summary>
/// PUT /regions/{code} — обновление региона. Требует роли Admin.
/// HTTP 404 — если регион не найден.
/// </summary>
public sealed class UpdateRegionEndpoint : IEndpoint<Ok<RegionModel>, UpdateRegionCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/regions/{code}",
                async (
                    string code,
                    UpdateRegionRequest body,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new UpdateRegionCommand(code, body.NameRu, body.NameEn, body.IsActive),
                        sender,
                        ct
                    )
            )
            .WithName("UpdateRegion")
            .WithTags("Admin.Regions")
            .WithSummary("Обновить регион")
            .WithDescription("Изменяет поля региона. Доступно только администраторам.")
            .Produces<RegionModel>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    /// <inheritdoc/>
    public async Task<Ok<RegionModel>> HandleAsync(
        UpdateRegionCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
