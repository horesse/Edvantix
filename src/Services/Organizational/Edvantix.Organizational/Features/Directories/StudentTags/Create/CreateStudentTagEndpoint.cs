namespace Edvantix.Organizational.Features.Directories.StudentTags.Create;

/// <summary>Эндпоинт создания тега студента.</summary>
public sealed class CreateStudentTagEndpoint
    : IEndpoint<Created<StudentTagDto>, CreateStudentTagCommand, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/tags",
                async (
                    CreateStudentTagCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("CreateStudentTag")
            .WithTags("Теги студентов")
            .WithSummary("Создать тег студента")
            .ProducesPost<StudentTagDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<Created<StudentTagDto>> HandleAsync(
        CreateStudentTagCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var dto = await sender.Send(command, cancellationToken);

        return TypedResults.Created($"/api/v1/directories/tags/{dto.Id}", dto);
    }
}
