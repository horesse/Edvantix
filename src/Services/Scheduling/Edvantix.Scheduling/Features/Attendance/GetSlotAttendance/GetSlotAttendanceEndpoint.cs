using Edvantix.Constants.Permissions;

namespace Edvantix.Scheduling.Features.Attendance.GetSlotAttendance;

/// <summary>
/// GET /v1/slots/{slotId}/attendance — returns all attendance records for a lesson slot.
/// <para>
/// Authorization gate: <c>scheduling.view-schedule</c> (D-09). Reuses the existing
/// view-schedule permission so managers and teachers with schedule visibility can also
/// read attendance without requiring a separate permission assignment.
/// </para>
/// </summary>
public sealed class GetSlotAttendanceEndpoint
    : IEndpoint<Ok<IReadOnlyList<AttendanceRecordDto>>, Guid, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/slots/{slotId:guid}/attendance",
                async (Guid slotId, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(slotId, sender, cancellationToken)
            )
            .Produces<IReadOnlyList<AttendanceRecordDto>>(StatusCodes.Status200OK)
            .WithName("GetSlotAttendance")
            .WithTags("Attendance")
            .WithSummary("Get slot attendance")
            .WithDescription(
                "Returns all attendance records for the specified lesson slot. "
                    + "Each record includes the student identifier, attendance status, "
                    + "correlation identifier, and the timestamp when the attendance was marked."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(SchedulingPermissions.ViewSchedule);
    }

    /// <inheritdoc/>
    public async Task<Ok<IReadOnlyList<AttendanceRecordDto>>> HandleAsync(
        Guid slotId,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetSlotAttendanceQuery(slotId), cancellationToken);

        return TypedResults.Ok(result);
    }
}
