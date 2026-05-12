namespace Edvantix.Curriculum.Features.Courses.Create;

/// <summary>POST /api/v1/courses — создать новый курс.</summary>
public sealed class CreateCourseEndpoint
    : IEndpoint<Created<Guid>, CreateCourseCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/courses",
                async (
                    CreateCourseCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .WithName("CreateCourse")
            .WithTags("Курсы")
            .WithSummary("Создать новый курс")
            .Produces<Guid>(StatusCodes.Status201Created)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateCourseCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location = linker.GetPathByName("GetCourseById", new { id }) ?? $"/api/v1/courses/{id}";
        return TypedResults.Created(location, id);
    }
}
