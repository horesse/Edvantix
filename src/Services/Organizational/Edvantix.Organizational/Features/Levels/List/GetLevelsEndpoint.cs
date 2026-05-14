namespace Edvantix.Organizational.Features.Levels.List;

public sealed class GetLevelsEndpoint
    : IEndpoint<Ok<IReadOnlyList<LevelDto>>, GetLevelsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/levels", HandleAsync)
            .WithName("GetLevels")
            .WithTags("Уровни")
            .WithSummary("Получить справочник уровней организации")
            .Produces<IReadOnlyList<LevelDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyList<LevelDto>>> HandleAsync(
        [AsParameters] GetLevelsQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);

        return TypedResults.Ok(result);
    }
}
