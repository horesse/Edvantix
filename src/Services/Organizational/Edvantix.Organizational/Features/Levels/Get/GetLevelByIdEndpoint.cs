namespace Edvantix.Organizational.Features.Levels.Get;

public sealed class GetLevelByIdEndpoint : IEndpoint<Ok<LevelDto>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/levels/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetLevelById")
            .WithTags("Уровни")
            .WithSummary("Получить уровень по идентификатору")
            .Produces<LevelDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<LevelDto>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetLevelByIdQuery(id), cancellationToken);

        return TypedResults.Ok(result);
    }
}
