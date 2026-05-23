namespace Edvantix.Organizational.Features.Directories.StudentTags.Archive;

/// <summary>Эндпоинт архивации тега студента.</summary>
public sealed class ArchiveStudentTagEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/tags/{id:guid}/archive",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("ArchiveStudentTag")
            .WithTags("Теги студентов")
            .WithSummary("Архивировать тег студента")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc/>
    public async Task<NoContent> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new ArchiveStudentTagCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
