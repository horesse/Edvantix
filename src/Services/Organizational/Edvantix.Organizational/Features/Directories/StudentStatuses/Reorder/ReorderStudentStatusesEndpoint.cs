namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Reorder;

/// <summary>PATCH /api/v1/directories/student-statuses/reorder — переупорядочить статусы студентов.</summary>
public sealed class ReorderStudentStatusesEndpoint
    : IEndpoint<NoContent, ReorderStudentStatusesCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/student-statuses/reorder",
                async (
                    ReorderStudentStatusesCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("ReorderStudentStatuses")
            .WithTags("Справочник: Статусы студентов")
            .WithSummary("Изменить порядок статусов студентов")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ReorderStudentStatusesCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
