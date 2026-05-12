using Edvantix.Chassis.Endpoints;
using Edvantix.Schedule.Domain.Enums;
using Edvantix.Schedule.Features.GroupSchedules;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Schedule.Features.GroupSchedules.UpdateSettings;

internal sealed class UpdateGroupScheduleSettingsEndpoint
    : IEndpoint<NoContent, UpdateGroupScheduleSettingsRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/organizations/{organizationId:guid}/groups/{groupId:guid}/schedule",
                async (
                    Guid organizationId,
                    Guid groupId,
                    [FromBody] UpdateGroupScheduleSettingsRequest request,
                    ISender sender
                ) => await HandleAsync(request with { GroupId = groupId }, sender)
            )
            .ProducesPut()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateGroupScheduleSettingsRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(
            new UpdateGroupScheduleSettingsCommand(
                request.GroupId,
                request.Recurrence,
                request.LessonDurationMinutes,
                request.EndMode,
                request.EndDate,
                request.LessonCount,
                request.BiweeklyParity,
                request.SkipHolidays,
                request.NotifyStudents,
                request.Slots
            ),
            cancellationToken
        );

        return TypedResults.NoContent();
    }
}

public sealed record UpdateGroupScheduleSettingsRequest(
    Guid GroupId,
    RecurrenceType Recurrence,
    short LessonDurationMinutes,
    EndMode EndMode,
    DateOnly? EndDate,
    short? LessonCount,
    int? BiweeklyParity,
    bool SkipHolidays,
    bool NotifyStudents,
    IReadOnlyList<SlotRequest> Slots
);
