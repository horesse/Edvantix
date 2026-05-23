namespace Edvantix.Organizational.Features.Directories.StudentTags.GetById;

/// <summary>Эндпоинт получения тега студента по идентификатору.</summary>
public sealed class GetStudentTagByIdEndpoint
    : IEndpoint<Results<Ok<StudentTagDto>, NotFound>, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/directories/tags/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetStudentTagById")
            .WithTags("Теги студентов")
            .WithSummary("Получить тег студента по идентификатору")
            .ProducesGet<StudentTagDto>(hasNotFound: true)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Results<Ok<StudentTagDto>, NotFound>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var dto = await sender.Send(new GetStudentTagByIdQuery(id), cancellationToken);

            return TypedResults.Ok(dto);
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}
