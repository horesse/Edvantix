using Edvantix.Groups.Features.Directories.Levels;

namespace Edvantix.Groups.Features.Directories.Levels.GetById;

/// <summary>GET /api/v1/directories/levels/{id} — получить уровень по идентификатору.</summary>
public sealed class GetLevelDirectoryByIdEndpoint
    : IEndpoint<Results<Ok<LevelDirectoryDto>, NotFound>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/levels/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetLevelDirectoryById")
            .WithTags("Справочник: Уровни")
            .WithSummary("Получить запись справочника «Уровни» по идентификатору")
            .Produces<LevelDirectoryDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Results<Ok<LevelDirectoryDto>, NotFound>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(new GetLevelDirectoryByIdQuery(id), cancellationToken);
        return TypedResults.Ok(dto);
    }
}
