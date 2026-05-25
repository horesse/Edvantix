namespace Edvantix.Organizational.Features.Directories.LessonTypes.GetById;

/// <summary>GET /api/v1/directories/lesson-types/{id} — получить тип занятия по идентификатору.</summary>
public sealed class GetLessonTypeByIdEndpoint : IEndpoint<Ok<LessonTypeDto>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/lesson-types/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetLessonTypeById")
            .WithTags("Справочник: Типы занятий")
            .WithSummary("Получить тип занятия по идентификатору")
            .Produces<LessonTypeDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<LessonTypeDto>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetLessonTypeByIdQuery(id), cancellationToken);

        return TypedResults.Ok(result);
    }
}
