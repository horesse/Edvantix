namespace Edvantix.Organizational.Features.Directories.StudentTags.List;

/// <summary>Эндпоинт постраничного списка тегов студентов.</summary>
public sealed class ListStudentTagsEndpoint
    : IEndpoint<Ok<PagedResult<StudentTagListItemDto>>, ListStudentTagsQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/tags",
                async (
                    [AsParameters] ListStudentTagsQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("ListStudentTags")
            .WithTags("Теги студентов")
            .WithSummary("Получить список тегов студентов организации")
            .ProducesGet<PagedResult<StudentTagListItemDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<PagedResult<StudentTagListItemDto>>> HandleAsync(
        ListStudentTagsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
