using Edvantix.Groups.Features.Directories.Levels;

namespace Edvantix.Groups.Features.Directories.Levels.List;

/// <summary>GET /api/v1/directories/levels — список уровней.</summary>
public sealed class ListLevelsDirectoryEndpoint
    : IEndpoint<Ok<PagedResult<LevelDirectoryListItemDto>>, ListLevelsDirectoryQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/levels",
                async (
                    [AsParameters] ListLevelsDirectoryQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("ListLevelsDirectory")
            .WithTags("Справочник: Уровни")
            .WithSummary("Список записей справочника «Уровни»")
            .Produces<PagedResult<LevelDirectoryListItemDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<LevelDirectoryListItemDto>>> HandleAsync(
        ListLevelsDirectoryQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
