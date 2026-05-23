namespace Edvantix.Organizational.Features.Directories.StudentStatuses.GetById;

/// <summary>Эндпоинт получения статуса студента по идентификатору.</summary>
public sealed class GetStudentStatusByIdEndpoint
    : IEndpoint<Results<Ok<StudentStatusDto>, NotFound>, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/student-statuses/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetStudentStatusById")
            .WithTags("Статусы студентов")
            .WithSummary("Получить статус студента по идентификатору")
            .ProducesGet<StudentStatusDto>(hasNotFound: true)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Results<Ok<StudentStatusDto>, NotFound>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var dto = await sender.Send(new GetStudentStatusByIdQuery(id), cancellationToken);

            return TypedResults.Ok(dto);
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}
