namespace Edvantix.Groups.Features.Directories.Subjects.Restore;

public sealed class RestoreSubjectEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/subjects/{id:guid}/restore",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("RestoreSubject")
            .WithTags("Предметы")
            .WithSummary("Восстановить предмет из архива")
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
        await sender.Send(new RestoreSubjectCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
