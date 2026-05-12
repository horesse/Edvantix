using Edvantix.Chassis.Endpoints;
using Edvantix.Schedule.Domain.Enums;
using Edvantix.Schedule.Features.GroupSchedules;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Schedule.Features.GroupSchedules.Create;

internal sealed class CreateGroupScheduleEndpoint
    : IEndpoint<Created<Guid>, CreateGroupScheduleRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{organizationId:guid}/groups/{groupId:guid}/schedule",
                async (
                    Guid organizationId,
                    Guid groupId,
                    [FromBody] CreateGroupScheduleRequest request,
                    ISender sender
                ) =>
                    await HandleAsync(
                        request with
                        {
                            GroupId = groupId,
                            OrganizationId = organizationId,
                        },
                        sender
                    )
            )
            .ProducesPost<Guid>()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateGroupScheduleRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new CreateGroupScheduleCommand(
            request.GroupId,
            request.OrganizationId,
            request.Recurrence,
            request.LessonDurationMinutes,
            request.StartDate,
            request.EndMode,
            request.EndDate,
            request.LessonCount,
            request.BiweeklyParity,
            request.SkipHolidays,
            request.NotifyStudents,
            request.Slots,
            request.HolidayCountryCode
        );

        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/groups/{request.GroupId}/schedule", id);
    }
}

public sealed record CreateGroupScheduleRequest(
    Guid GroupId,
    Guid OrganizationId,
    RecurrenceType Recurrence,
    short LessonDurationMinutes,
    DateOnly StartDate,
    EndMode EndMode,
    DateOnly? EndDate,
    short? LessonCount,
    int? BiweeklyParity,
    bool SkipHolidays,
    bool NotifyStudents,
    IReadOnlyList<SlotRequest> Slots,
    string? HolidayCountryCode
);
