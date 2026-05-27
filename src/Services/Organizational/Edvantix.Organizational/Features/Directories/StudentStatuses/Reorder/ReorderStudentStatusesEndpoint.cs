using Edvantix.Organizational.Features.Directories;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Reorder;

/// <summary>PATCH /api/v1/directories/student-statuses/reorder — переупорядочить статусы студентов.</summary>
public sealed class ReorderStudentStatusesEndpoint : IEndpoint<NoContent, ReorderRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/directories/student-statuses/reorder",
                async (
                    ReorderRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("ReorderStudentStatuses")
            .WithTags("Статусы студентов")
            .WithSummary("Изменить порядок статусов студентов")
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
        await sender.Send(new ReorderStudentStatusesCommand(request.OrderedIds), cancellationToken);

        return TypedResults.NoContent();
    }
}
