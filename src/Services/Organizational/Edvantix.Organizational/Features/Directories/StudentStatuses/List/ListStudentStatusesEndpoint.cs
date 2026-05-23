namespace Edvantix.Organizational.Features.Directories.StudentStatuses.List;

/// <summary>Эндпоинт постраничного списка статусов студентов.</summary>
public sealed class ListStudentStatusesEndpoint
    : IEndpoint<Ok<PagedResult<StudentStatusListItemDto>>, ListStudentStatusesQuery, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/student-statuses",
                async (
                    [AsParameters] ListStudentStatusesQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("ListStudentStatuses")
            .WithTags("Статусы студентов")
            .WithSummary("Получить список статусов студентов организации")
            .ProducesGet<PagedResult<StudentStatusListItemDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<PagedResult<StudentStatusListItemDto>>> HandleAsync(
        ListStudentStatusesQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
