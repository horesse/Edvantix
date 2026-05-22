using Edvantix.Groups.Features.Directories.Levels;

namespace Edvantix.Groups.Features.Directories.Levels.Create;

/// <summary>POST /api/v1/directories/levels — создать уровень.</summary>
public sealed class CreateLevelDirectoryEndpoint
    : IEndpoint<Created<LevelDirectoryDto>, CreateLevelDirectoryCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/levels",
                async (
                    CreateLevelDirectoryCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .WithName("CreateLevelDirectory")
            .WithTags("Справочник: Уровни")
            .WithSummary("Создать запись в справочнике «Уровни»")
            .Produces<LevelDirectoryDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status409Conflict)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<LevelDirectoryDto>> HandleAsync(
        CreateLevelDirectoryCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);
        var location =
            linker.GetPathByName("GetLevelDirectoryById", new { id = dto.Id })
            ?? $"/api/directories/levels/{dto.Id}";

        return TypedResults.Created(location, dto);
    }
}
