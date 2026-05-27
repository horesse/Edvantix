using Edvantix.Organizational.Features.Directories;

namespace Edvantix.Organizational.Features.Directories.Subjects.Reorder;

/// <summary>PATCH /api/v1/directories/subjects/reorder — переупорядочить предметы.</summary>
public sealed class ReorderSubjectsEndpoint : IEndpoint<NoContent, ReorderRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/subjects/reorder",
                async (ReorderRequest request, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("ReorderSubjects")
            .WithTags("Справочник: Предметы")
            .WithSummary("Изменить порядок предметов")
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
        await sender.Send(new ReorderSubjectsCommand(request.OrderedIds), cancellationToken);

        return TypedResults.NoContent();
    }
}
