namespace Edvantix.Organizational.Features.Directories.Subjects.Create;

/// <summary>POST /api/v1/directories/subjects — создать предмет.</summary>
public sealed class CreateSubjectEndpoint
    : IEndpoint<Created<Guid>, CreateSubjectCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/subjects",
                async (
                    CreateSubjectCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .WithName("CreateSubject")
            .WithTags("Справочник: Предметы")
            .WithSummary("Создать предмет в справочнике организации")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateSubjectCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location =
            linker.GetPathByName("GetSubjectById", new { id }) ?? $"/api/directories/subjects/{id}";

        return TypedResults.Created(location, id);
    }
}
