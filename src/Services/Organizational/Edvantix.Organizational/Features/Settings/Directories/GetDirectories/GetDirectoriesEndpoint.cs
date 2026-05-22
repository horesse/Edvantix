using Edvantix.Constants.Core;

namespace Edvantix.Organizational.Features.Settings.Directories.GetDirectories;

/// <summary>
/// Endpoint: GET /api/v1/settings/directories
/// Возвращает каталог справочников настроек — ровно 8 элементов в фиксированном порядке.
/// </summary>
public sealed class GetDirectoriesEndpoint
    : IEndpoint<Ok<IReadOnlyList<DirectorySummaryDto>>, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/settings/directories",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(sender, cancellationToken)
            )
            .WithName("GetDirectories")
            .WithTags("Настройки")
            .WithSummary("Каталог справочников")
            .WithDescription(
                "Возвращает список всех 8 справочников настроек с их статистикой. "
                    + "Ответ содержит ровно 8 элементов в фиксированном порядке. "
                    + "Справочники без реализации возвращают IsAvailable=false."
            )
            .Produces<IReadOnlyList<DirectorySummaryDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyList<DirectorySummaryDto>>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetDirectoriesQuery(), cancellationToken);
        return TypedResults.Ok(result);
    }
}
