namespace Edvantix.Organizational.Features.Directories.LessonTypes.Reorder;

/// <summary>PATCH /api/v1/directories/lesson-types/reorder — переупорядочить типы занятий.</summary>
public sealed class ReorderLessonTypesEndpoint
    : IEndpoint<NoContent, ReorderLessonTypesCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/lesson-types/reorder",
                async (
                    ReorderLessonTypesCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("ReorderLessonTypes")
            .WithTags("Справочник: Типы занятий")
            .WithSummary("Изменить порядок типов занятий")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ReorderLessonTypesCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
