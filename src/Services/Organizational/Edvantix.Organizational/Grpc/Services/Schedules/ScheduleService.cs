using Edvantix.Schedule.Grpc.Services;

namespace Edvantix.Organizational.Grpc.Services.Schedules;

[ExcludeFromCodeCoverage]
internal sealed class ScheduleService(ScheduleGrpcService.ScheduleGrpcServiceClient client)
    : IScheduleService
{
    public async Task<IReadOnlyDictionary<Guid, ScheduleSummaryDto>> GetScheduleSummariesAsync(
        IEnumerable<Guid> groupIds,
        CancellationToken cancellationToken = default
    )
    {
        var request = new GetScheduleSummariesRequest();
        request.GroupIds.AddRange(groupIds.Select(id => id.ToString()));

        var response = await client.GetScheduleSummariesByGroupIdsAsync(
            request,
            cancellationToken: cancellationToken
        );

        return response.Summaries.ToDictionary(
            s => Guid.Parse(s.GroupId),
            s => new ScheduleSummaryDto(
                SummaryText: s.SummaryText,
                LessonDurationMinutes: s.LessonDurationMinutes,
                NextLessonDate: string.IsNullOrEmpty(s.NextLessonDate)
                    ? null
                    : DateOnly.Parse(s.NextLessonDate),
                LessonCountTotal: s.LessonCountTotal,
                LessonCountRemaining: s.LessonCountRemaining
            )
        );
    }

    public async Task<ScheduleDetailDto?> GetScheduleByGroupIdAsync(
        Guid groupId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new GetScheduleByGroupIdRequest { GroupId = groupId.ToString() };
        var response = await client.GetScheduleByGroupIdAsync(
            request,
            cancellationToken: cancellationToken
        );

        if (!response.Found || response.Schedule is null)
            return null;

        var s = response.Schedule;

        return new ScheduleDetailDto(
            Id: Guid.Parse(s.Id),
            Recurrence: s.Recurrence,
            BiweeklyParity: s.HasBiweeklyParity ? s.BiweeklyParity : null,
            LessonDurationMinutes: (short)s.LessonDurationMinutes,
            StartDate: DateOnly.Parse(s.StartDate),
            EndMode: s.EndMode,
            EndDate: s.HasEndDate ? DateOnly.Parse(s.EndDate) : null,
            LessonCount: s.HasLessonCount ? (short)s.LessonCount : null,
            SkipHolidays: s.SkipHolidays,
            Slots: s.Slots
                .Select(slot => new ScheduleSlotDto(slot.Weekday, slot.StartMinutes))
                .ToList(),
            Exceptions: s.Exceptions
                .Select(ex => new ScheduleExceptionDto(
                    DateOnly.Parse(ex.Date),
                    string.IsNullOrEmpty(ex.Reason) ? null : ex.Reason
                ))
                .ToList(),
            SummaryText: s.SummaryText
        );
    }

    public async Task<IReadOnlyList<UpcomingLessonDto>> GetUpcomingLessonsAsync(
        Guid groupId,
        int count = 5,
        CancellationToken cancellationToken = default
    )
    {
        var request = new GetUpcomingLessonsRequest
        {
            GroupId = groupId.ToString(),
            Count = count,
        };

        var response = await client.GetUpcomingLessonsAsync(
            request,
            cancellationToken: cancellationToken
        );

        return response.Lessons
            .Select(l =>
            {
                var startTime = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(l.StartMinutes));
                var endTime = TimeOnly.FromTimeSpan(
                    TimeSpan.FromMinutes(l.StartMinutes + l.DurationMinutes)
                );

                return new UpcomingLessonDto(DateOnly.Parse(l.Date), startTime, endTime);
            })
            .ToList();
    }
}
