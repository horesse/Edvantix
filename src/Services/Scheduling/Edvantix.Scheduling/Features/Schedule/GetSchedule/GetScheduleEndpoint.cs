using Edvantix.Constants.Permissions;

namespace Edvantix.Scheduling.Features.Schedule.GetSchedule;

/// <summary>
/// GET /v1/schedule — returns lesson slots for the current tenant filtered by the caller's permission level.
/// <para>
/// Baseline authorization gate: <c>scheduling.view-schedule</c>.
/// Data-level filtering (manager vs teacher vs student) is applied inside
/// <see cref="GetScheduleQueryHandler"/> via two additional gRPC permission checks.
/// </para>
/// </summary>
public sealed class GetScheduleEndpoint
    : IEndpoint<Ok<List<ScheduleSlotDto>>, DateTimeOffset, DateTimeOffset, ISender>
{
    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/schedule",
                async (
                    [AsParameters] DateTimeOffset dateFrom,
                    [AsParameters] DateTimeOffset dateTo,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(dateFrom, dateTo, sender, cancellationToken)
            )
            .Produces<List<ScheduleSlotDto>>(StatusCodes.Status200OK)
            .WithName("GetSchedule")
            .WithTags("Schedule")
            .WithSummary("Get schedule")
            .WithDescription(
                "Returns lesson slots for the current school within the requested date range. "
                    + "Managers see all groups' slots with teacher identity and student counts. "
                    + "Teachers see only their own slots with student counts. "
                    + "Students see only slots for groups they belong to, resolved via Organizations service."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(SchedulingPermissions.ViewSchedule);
    }

    /// <inheritdoc/>
    public async Task<Ok<List<ScheduleSlotDto>>> HandleAsync(
        DateTimeOffset dateFrom,
        DateTimeOffset dateTo,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetScheduleQuery(dateFrom, dateTo), cancellationToken);

        return TypedResults.Ok(result);
    }
}
