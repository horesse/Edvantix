namespace Edvantix.Groups.Features.Directories.Subjects.Update;

public sealed class UpdateSubjectEndpoint : IEndpoint<Ok<SubjectDto>, UpdateSubjectCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/directories/subjects/{id:guid}",
                async (
                    Guid id,
                    UpdateSubjectCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    // id из маршрута имеет приоритет над id из тела
                    var merged = command with
                    {
                        Id = id,
                    };
                    return await HandleAsync(merged, sender, cancellationToken);
                }
            )
            .WithName("UpdateSubject")
            .WithTags("Предметы")
            .WithSummary("Обновить данные предмета в справочнике")
            .Produces<SubjectDto>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<SubjectDto>> HandleAsync(
        UpdateSubjectCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        var dto = await sender.Send(new GetById.GetSubjectByIdQuery(command.Id), cancellationToken);

        return TypedResults.Ok(dto);
    }
}
