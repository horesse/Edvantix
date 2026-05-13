namespace Edvantix.Curriculum.Features.Modules.Delete;

/// <summary>DELETE /api/v1/courses/{courseId}/modules/{moduleId} — удалить модуль из курса.</summary>
public sealed class DeleteModuleEndpoint : IEndpoint<NoContent, DeleteModuleCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/courses/{courseId:guid}/modules/{moduleId:guid}",
                async (
                    Guid courseId,
                    Guid moduleId,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new DeleteModuleCommand(courseId, moduleId),
                        sender,
                        cancellationToken
                    )
            )
            .WithTags("Модули")
            .WithSummary("Удалить модуль из курса")
            .Produces(StatusCodes.Status204NoContent)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        DeleteModuleCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
