namespace Edvantix.Catalog.Features.RegionFeature.CreateRegion;

/// <summary>
/// POST /regions — создание региона. Требует роли Admin.
/// HTTP 201 — успех, HTTP 400 — невалидные данные, HTTP 409 — код уже занят.
/// </summary>
public sealed class CreateRegionEndpoint
    : IEndpoint<Created<RegionModel>, CreateRegionCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/regions",
                async (CreateRegionCommand req, ISender sender, CancellationToken ct) =>
                    await HandleAsync(req, sender, ct)
            )
            .WithName("CreateRegion")
            .WithTags("Admin.Regions")
            .WithSummary("Создать регион")
            .WithDescription(
                "Добавляет новый регион в справочник. Доступно только администраторам."
            )
            .Produces<RegionModel>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    /// <inheritdoc/>
    public async Task<Created<RegionModel>> HandleAsync(
        CreateRegionCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/regions/{result.Code}", result);
    }
}
