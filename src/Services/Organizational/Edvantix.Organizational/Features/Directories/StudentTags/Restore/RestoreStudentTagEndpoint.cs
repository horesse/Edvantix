namespace Edvantix.Organizational.Features.Directories.StudentTags.Restore;

/// <summary>Эндпоинт восстановления тега студента из архива.</summary>
public sealed class RestoreStudentTagEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/tags/{id:guid}/restore",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("RestoreStudentTag")
            .WithTags("Теги студентов")
            .WithSummary("Восстановить тег студента из архива")
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
        await sender.Send(new RestoreStudentTagCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
