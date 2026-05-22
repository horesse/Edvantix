namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Update;

/// <summary>Эндпоинт обновления статуса студента.</summary>
public sealed class UpdateStudentStatusEndpoint
    : IEndpoint<Ok<StudentStatusDto>, UpdateStudentStatusCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/directories/student-statuses/{id:guid}",
                async (
                    UpdateStudentStatusCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("UpdateStudentStatus")
            .WithTags("Статусы студентов")
            .WithSummary("Обновить статус студента")
            .Produces<StudentStatusDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Ok<StudentStatusDto>> HandleAsync(
        UpdateStudentStatusCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);

        return TypedResults.Ok(dto);
    }
}
