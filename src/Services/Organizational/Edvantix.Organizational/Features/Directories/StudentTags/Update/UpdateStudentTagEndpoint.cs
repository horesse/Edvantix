namespace Edvantix.Organizational.Features.Directories.StudentTags.Update;

/// <summary>Эндпоинт обновления тега студента.</summary>
public sealed class UpdateStudentTagEndpoint
    : IEndpoint<Ok<StudentTagDto>, UpdateStudentTagCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/directories/tags/{id:guid}",
                async (
                    UpdateStudentTagCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("UpdateStudentTag")
            .WithTags("Теги студентов")
            .WithSummary("Обновить тег студента")
            .Produces<StudentTagDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<StudentTagDto>> HandleAsync(
        UpdateStudentTagCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);

        return TypedResults.Ok(dto);
    }
}
