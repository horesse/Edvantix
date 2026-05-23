namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Restore;

/// <summary>Эндпоинт восстановления статуса студента из архива.</summary>
public sealed class RestoreStudentStatusEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/student-statuses/{id:guid}/restore",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("RestoreStudentStatus")
            .WithTags("Статусы студентов")
            .WithSummary("Восстановить статус студента из архива")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
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
        await sender.Send(new RestoreStudentStatusCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
