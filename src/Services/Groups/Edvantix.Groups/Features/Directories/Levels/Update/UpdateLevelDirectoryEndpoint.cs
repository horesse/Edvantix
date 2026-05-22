using Edvantix.Groups.Features.Directories.Levels;

namespace Edvantix.Groups.Features.Directories.Levels.Update;

/// <summary>PUT /api/v1/directories/levels/{id} — обновить уровень.</summary>
public sealed class UpdateLevelDirectoryEndpoint
    : IEndpoint<Results<Ok<LevelDirectoryDto>, NotFound>, UpdateLevelDirectoryCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/directories/levels/{id:guid}",
                async (
                    Guid id,
                    UpdateLevelDirectoryCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var cmd = command with { Id = id };
                    return await HandleAsync(cmd, sender, cancellationToken);
                }
            )
            .WithName("UpdateLevelDirectory")
            .WithTags("Справочник: Уровни")
            .WithSummary("Обновить запись в справочнике «Уровни»")
            .Produces<LevelDirectoryDto>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Results<Ok<LevelDirectoryDto>, NotFound>> HandleAsync(
        UpdateLevelDirectoryCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(dto);
    }
}
