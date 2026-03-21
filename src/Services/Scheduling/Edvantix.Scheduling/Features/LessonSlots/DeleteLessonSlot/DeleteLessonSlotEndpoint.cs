using Edvantix.Constants.Permissions;

namespace Edvantix.Scheduling.Features.LessonSlots.DeleteLessonSlot;

/// <summary>DELETE /v1/lesson-slots/{id} — hard-deletes a lesson slot.</summary>
public sealed class DeleteLessonSlotEndpoint
    : IEndpoint<NoContent, DeleteLessonSlotCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/lesson-slots/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(
                        new DeleteLessonSlotCommand { Id = id },
                        sender,
                        cancellationToken
                    )
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteLessonSlot")
            .WithTags("LessonSlots")
            .WithSummary("Delete a lesson slot")
            .WithDescription(
                "Hard-deletes a lesson slot belonging to the current school. Returns 404 if the slot does not exist."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(SchedulingPermissions.DeleteLessonSlot);
    }

    public async Task<NoContent> HandleAsync(
        DeleteLessonSlotCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
