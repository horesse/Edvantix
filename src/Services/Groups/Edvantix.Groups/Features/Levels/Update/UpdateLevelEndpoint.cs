namespace Edvantix.Groups.Features.Levels.Update;

public sealed class UpdateLevelEndpoint : IEndpoint<NoContent, UpdateLevelCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/levels/{id:guid}",
                async (
                    Guid id,
                    UpdateLevelCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command with { Id = id }, sender, cancellationToken)
            )
            .WithName("UpdateLevel")
            .WithTags("Уровни")
            .WithSummary("Обновить данные уровня")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateLevelCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
