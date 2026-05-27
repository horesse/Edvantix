using Edvantix.Organizational.Features.Directories;

namespace Edvantix.Organizational.Features.Directories.LessonTypes.Reorder;

/// <summary>PATCH /api/v1/directories/lesson-types/reorder — переупорядочить типы занятий.</summary>
public sealed class ReorderLessonTypesEndpoint : IEndpoint<NoContent, ReorderRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/lesson-types/reorder",
                async (ReorderRequest request, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(request, sender, cancellationToken)
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
        ReorderRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new ReorderLessonTypesCommand(request.OrderedIds), cancellationToken);

        return TypedResults.NoContent();
    }
}
