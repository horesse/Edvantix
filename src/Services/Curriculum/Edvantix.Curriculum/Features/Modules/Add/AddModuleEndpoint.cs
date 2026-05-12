namespace Edvantix.Curriculum.Features.Modules.Add;

/// <summary>POST /api/v1/courses/{courseId}/modules — добавить модуль в курс.</summary>
public sealed class AddModuleEndpoint : IEndpoint<Created<Guid>, AddModuleCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/courses/{courseId:guid}/modules",
                async (
                    Guid courseId,
                    AddModuleCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        command with
                        {
                            CourseId = courseId,
                        },
                        sender,
                        cancellationToken
                    )
            )
            .WithTags("Модули")
            .WithSummary("Добавить модуль в курс")
            .Produces<Guid>(StatusCodes.Status201Created)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        AddModuleCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/courses/{command.CourseId}/modules/{id}", id);
    }
}
