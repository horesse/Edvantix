using Edvantix.Constants.Permissions;

namespace Edvantix.Scheduling.Features.LessonSlots.EditLessonSlot;

/// <summary>PUT /v1/lesson-slots/{id} — edits the teacher or time range of an existing lesson slot.</summary>
public sealed class EditLessonSlotEndpoint : IEndpoint<NoContent, EditLessonSlotCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/lesson-slots/{id:guid}",
                async (
                    Guid id,
                    EditLessonSlotRequest body,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new EditLessonSlotCommand
                        {
                            Id = id,
                            TeacherId = body.TeacherId,
                            StartTime = body.StartTime,
                            EndTime = body.EndTime,
                        },
                        sender,
                        cancellationToken
                    )
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("EditLessonSlot")
            .WithTags("LessonSlots")
            .WithSummary("Edit a lesson slot")
            .WithDescription(
                "Updates the teacher or time range of a lesson slot. "
                    + "Returns 422 if the teacher is already booked at the new time. "
                    + "The current slot is excluded from the conflict check (self-exclusion per D-06)."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(SchedulingPermissions.EditLessonSlot);
    }

    public async Task<NoContent> HandleAsync(
        EditLessonSlotCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}

/// <summary>Request body for the edit-lesson-slot endpoint.</summary>
public sealed record EditLessonSlotRequest(
    Guid TeacherId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime
);
