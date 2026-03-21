using Edvantix.Constants.Permissions;
using Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

namespace Edvantix.Scheduling.Features.Attendance.MarkAttendance;

/// <summary>PUT /v1/slots/{slotId}/attendance/{studentId} — marks or updates a student's attendance (upsert).</summary>
public sealed class MarkAttendanceEndpoint : IEndpoint
{
    /// <summary>Request body for marking attendance.</summary>
    public sealed record MarkAttendanceRequest(AttendanceStatus Status);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/slots/{slotId:guid}/attendance/{studentId:guid}",
                async (
                    Guid slotId,
                    Guid studentId,
                    MarkAttendanceRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    await sender.Send(
                        new MarkAttendanceCommand(slotId, studentId, request.Status),
                        cancellationToken
                    );

                    return TypedResults.Ok();
                }
            )
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("MarkAttendance")
            .WithTags("Attendance")
            .WithSummary("Mark student attendance")
            .WithDescription(
                "Marks or updates a student's attendance for a lesson slot (upsert). "
                    + "Returns 200 OK regardless of whether a new record was created or an existing one was updated."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(SchedulingPermissions.MarkAttendance);
    }
}
