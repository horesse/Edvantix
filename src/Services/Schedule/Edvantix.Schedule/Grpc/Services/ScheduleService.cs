using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;
using Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;
using Edvantix.Schedule.Domain.Services;
using Grpc.Core;
using Microsoft.AspNetCore.RateLimiting;

namespace Edvantix.Schedule.Grpc.Services;

/// <summary>
/// gRPC-сервис расписаний.
/// Используется Organizational-сервисом для обогащения групп данными расписания.
/// </summary>
internal sealed class ScheduleService(
    IGroupScheduleRepository scheduleRepository,
    ILessonOccurrenceRepository occurrenceRepository
) : ScheduleGrpcService.ScheduleGrpcServiceBase
{
    [EnableRateLimiting("PerUserRateLimit")]
    public override async Task<GetScheduleSummariesResponse> GetScheduleSummariesByGroupIds(
        GetScheduleSummariesRequest request,
        ServerCallContext context
    )
    {
        var groupIds = ParseGroupIds(request.GroupIds);

        if (groupIds.Count == 0)
            return new GetScheduleSummariesResponse();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var (schedules, occurrenceSummaries) = await FetchDataAsync(
            groupIds,
            today,
            context.CancellationToken
        );

        var scheduleByGroupId = schedules.ToDictionary(s => s.GroupId);

        var response = new GetScheduleSummariesResponse();

        foreach (var groupId in groupIds)
        {
            var summary = BuildSummary(groupId, scheduleByGroupId, occurrenceSummaries);
            response.Summaries.Add(summary);
        }

        return response;
    }

    [EnableRateLimiting("PerUserRateLimit")]
    public override async Task<GetScheduleByGroupIdResponse> GetScheduleByGroupId(
        GetScheduleByGroupIdRequest request,
        ServerCallContext context
    )
    {
        if (!Guid.TryParse(request.GroupId, out var groupId))
            return new GetScheduleByGroupIdResponse { Found = false };

        var schedule = await scheduleRepository.GetByGroupIdAsync(
            groupId,
            context.CancellationToken
        );

        if (schedule is null)
            return new GetScheduleByGroupIdResponse { Found = false };

        return new GetScheduleByGroupIdResponse
        {
            Found = true,
            Schedule = BuildScheduleDetail(schedule),
        };
    }

    [EnableRateLimiting("PerUserRateLimit")]
    public override async Task<GetUpcomingLessonsResponse> GetUpcomingLessons(
        GetUpcomingLessonsRequest request,
        ServerCallContext context
    )
    {
        if (!Guid.TryParse(request.GroupId, out var groupId))
            return new GetUpcomingLessonsResponse();

        var count = request.Count > 0 ? request.Count : 5;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var occurrences = await occurrenceRepository.GetUpcomingByGroupIdAsync(
            groupId,
            today,
            count,
            context.CancellationToken
        );

        var response = new GetUpcomingLessonsResponse();

        response.Lessons.AddRange(
            occurrences.Select(o => new UpcomingLessonProto
            {
                Date = o.LessonDate.ToString("yyyy-MM-dd"),
                StartMinutes = o.StartMinutes,
                DurationMinutes = o.DurationMinutes,
            })
        );

        return response;
    }

    // ── private helpers ───────────────────────────────────────────────────────

    private static List<Guid> ParseGroupIds(IEnumerable<string> rawIds)
    {
        var result = new List<Guid>();

        foreach (var id in rawIds)
        {
            if (Guid.TryParse(id, out var guid))
                result.Add(guid);
        }

        return result;
    }

    private async Task<(
        IReadOnlyList<GroupSchedule> Schedules,
        IReadOnlyDictionary<Guid, OccurrenceSummary> OccurrenceSummaries
    )> FetchDataAsync(List<Guid> groupIds, DateOnly today, CancellationToken ct)
    {
        var schedulesTask = scheduleRepository.GetByGroupIdsAsync(groupIds, ct);
        var summariesTask = occurrenceRepository.GetSummariesByGroupIdsAsync(groupIds, today, ct);

        await Task.WhenAll(schedulesTask, summariesTask);

        return (schedulesTask.Result, summariesTask.Result);
    }

    private static ScheduleSummary BuildSummary(
        Guid groupId,
        Dictionary<Guid, GroupSchedule> scheduleByGroupId,
        IReadOnlyDictionary<Guid, OccurrenceSummary> occurrenceSummaries
    )
    {
        var proto = new ScheduleSummary { GroupId = groupId.ToString() };

        if (scheduleByGroupId.TryGetValue(groupId, out var schedule))
        {
            proto.LessonDurationMinutes = schedule.LessonDurationMinutes;
            proto.SummaryText = ScheduleSummaryFormatter.Format(
                schedule.Slots.Select(s => (s.Weekday, s.StartMinutes)),
                schedule.LessonDurationMinutes
            );
        }

        if (occurrenceSummaries.TryGetValue(groupId, out var occ))
        {
            proto.LessonCountTotal = occ.Total;
            proto.LessonCountRemaining = occ.Remaining;
            proto.NextLessonDate = occ.NextLessonDate?.ToString("yyyy-MM-dd") ?? string.Empty;
        }

        return proto;
    }

    private static ScheduleDetail BuildScheduleDetail(GroupSchedule schedule)
    {
        var detail = new ScheduleDetail
        {
            Id = schedule.Id.ToString(),
            Recurrence = schedule.Recurrence.ToString(),
            LessonDurationMinutes = schedule.LessonDurationMinutes,
            StartDate = schedule.StartDate.ToString("yyyy-MM-dd"),
            EndMode = schedule.EndMode.ToString(),
            SkipHolidays = schedule.SkipHolidays,
            SummaryText = ScheduleSummaryFormatter.Format(
                schedule.Slots.Select(s => (s.Weekday, s.StartMinutes)),
                schedule.LessonDurationMinutes
            ),
        };

        if (schedule.BiweeklyParity.HasValue)
            detail.BiweeklyParity = schedule.BiweeklyParity.Value;

        if (schedule.EndDate.HasValue)
            detail.EndDate = schedule.EndDate.Value.ToString("yyyy-MM-dd");

        if (schedule.LessonCount.HasValue)
            detail.LessonCount = schedule.LessonCount.Value;

        detail.Slots.AddRange(
            schedule.Slots.Select(s => new ScheduleSlotProto
            {
                Weekday = s.Weekday,
                StartMinutes = s.StartMinutes,
            })
        );

        detail.Exceptions.AddRange(
            schedule.Exceptions.Select(e => new ScheduleExceptionProto
            {
                Date = e.ExceptionDate.ToString("yyyy-MM-dd"),
                Reason = e.Reason ?? string.Empty,
            })
        );

        return detail;
    }
}
