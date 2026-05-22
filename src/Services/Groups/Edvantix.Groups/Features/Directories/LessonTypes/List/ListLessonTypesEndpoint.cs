namespace Edvantix.Groups.Features.Directories.LessonTypes.List;

/// <summary>GET /api/v1/directories/lesson-types — список типов занятий организации.</summary>
public sealed class ListLessonTypesEndpoint
    : IEndpoint<Ok<PagedResult<LessonTypeListItemDto>>, ListLessonTypesQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/directories/lesson-types", HandleAsync)
            .WithName("ListLessonTypes")
            .WithTags("Типы занятий")
            .WithSummary("Получить справочник типов занятий организации")
            .Produces<PagedResult<LessonTypeListItemDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<LessonTypeListItemDto>>> HandleAsync(
        [AsParameters] ListLessonTypesQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);

        return TypedResults.Ok(result);
    }
}
