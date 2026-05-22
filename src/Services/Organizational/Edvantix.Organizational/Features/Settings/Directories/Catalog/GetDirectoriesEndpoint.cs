using Edvantix.Constants.Core;

namespace Edvantix.Organizational.Features.Settings.Directories.Catalog;

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
            .WithSummary("Каталог справочников организации")
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
