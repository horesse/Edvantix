namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Create;

/// <summary>Эндпоинт создания статуса студента.</summary>
public sealed class CreateStudentStatusEndpoint
    : IEndpoint<Created<StudentStatusDto>, CreateStudentStatusCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/student-statuses",
                async (
                    CreateStudentStatusCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("CreateStudentStatus")
            .WithTags("Статусы студентов")
            .WithSummary("Создать статус студента")
            .ProducesPost<StudentStatusDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Created<StudentStatusDto>> HandleAsync(
        CreateStudentStatusCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);

        return TypedResults.Created(
            $"/api/v1/directories/student-statuses/{dto.Id}",
            dto
        );
    }
}
