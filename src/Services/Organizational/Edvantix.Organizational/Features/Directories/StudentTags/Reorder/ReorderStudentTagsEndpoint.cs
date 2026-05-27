using Edvantix.Organizational.Features.Directories;

namespace Edvantix.Organizational.Features.Directories.StudentTags.Reorder;

/// <summary>PATCH /api/v1/directories/tags/reorder — переупорядочить теги студентов.</summary>
public sealed class ReorderStudentTagsEndpoint : IEndpoint<NoContent, ReorderRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/tags/reorder",
                async (
                    ReorderRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("ReorderStudentTags")
            .WithTags("Теги студентов")
            .WithSummary("Изменить порядок тегов студентов")
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
        await sender.Send(new ReorderStudentTagsCommand(request.OrderedIds), cancellationToken);

        return TypedResults.NoContent();
    }
}
