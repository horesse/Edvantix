namespace Edvantix.Organizational.Features.Directories.Subjects.Archive;

/// <summary>POST /api/v1/directories/subjects/{id}/archive — архивировать предмет.</summary>
public sealed class ArchiveSubjectEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/subjects/{id:guid}/archive",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("ArchiveSubject")
            .WithTags("Справочник: Предметы")
            .WithSummary("Архивировать предмет в справочнике")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new ArchiveSubjectCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
