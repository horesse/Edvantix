using Edvantix.Constants.Permissions;

namespace Edvantix.Scheduling.Features.LessonSlots.CreateLessonSlot;

/// <summary>POST /v1/lesson-slots — creates a new lesson slot for the current tenant.</summary>
public sealed class CreateLessonSlotEndpoint
    : IEndpoint<Created<Guid>, CreateLessonSlotCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/lesson-slots",
                async (
                    CreateLessonSlotCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("CreateLessonSlot")
            .WithTags("LessonSlots")
            .WithSummary("Create a lesson slot")
            .WithDescription(
                "Creates a new lesson slot binding a group, teacher, and time range within the current school. "
                    + "Returns 404 if the group does not exist in the Organizations service. "
                    + "Returns 422 if the teacher is already booked at the requested time."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(SchedulingPermissions.CreateLessonSlot);
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateLessonSlotCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location =
            linker.GetPathByName("GetLessonSlotById", new { id }) ?? $"/api/v1/lesson-slots/{id}";

        return TypedResults.Created(location, id);
    }
}
