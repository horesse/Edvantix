namespace Edvantix.Curriculum.Features.Lessons.Add;

/// <summary>POST /api/v1/modules/{moduleId}/lessons — добавить урок в модуль.</summary>
public sealed class AddLessonEndpoint : IEndpoint<Created<Guid>, AddLessonCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/modules/{moduleId:guid}/lessons",
                async (
                    Guid moduleId,
                    AddLessonCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        command with
                        {
                            ModuleId = moduleId,
                        },
                        sender,
                        cancellationToken
                    )
            )
            .WithTags("Уроки")
            .WithSummary("Добавить урок в модуль")
            .Produces<Guid>(StatusCodes.Status201Created)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        AddLessonCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/modules/{command.ModuleId}/lessons/{id}", id);
    }
}
