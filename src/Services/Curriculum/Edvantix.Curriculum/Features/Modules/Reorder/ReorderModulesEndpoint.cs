namespace Edvantix.Curriculum.Features.Modules.Reorder;

/// <summary>PUT /api/v1/courses/{courseId}/modules/reorder — переупорядочить модули курса.</summary>
public sealed class ReorderModulesEndpoint : IEndpoint<NoContent, ReorderModulesCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/courses/{courseId:guid}/modules/reorder",
                async (
                    Guid courseId,
                    ReorderModulesCommand command,
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
            .WithSummary("Переупорядочить модули курса")
            .Produces(StatusCodes.Status204NoContent)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ReorderModulesCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
