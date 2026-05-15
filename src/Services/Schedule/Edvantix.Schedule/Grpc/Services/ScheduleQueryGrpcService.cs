using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;
using Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;
using Edvantix.Schedule.Domain.Services;
using Grpc.Core;
using Microsoft.AspNetCore.RateLimiting;

namespace Edvantix.Schedule.Grpc.Services;

/// <summary>
/// gRPC-сервис расписаний.
/// Используется Organizational-сервисом для обогащения списка групп сводкой расписания.
/// </summary>
internal sealed class ScheduleQueryGrpcService(
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

    // Task 8 — стаб, будет реализован отдельно.
    public override Task<GetScheduleByGroupIdResponse> GetScheduleByGroupId(
        GetScheduleByGroupIdRequest request,
        ServerCallContext context
    ) => Task.FromResult(new GetScheduleByGroupIdResponse { Found = false });

    // Task 8 — стаб, будет реализован отдельно.
    public override Task<GetUpcomingLessonsResponse> GetUpcomingLessons(
        GetUpcomingLessonsRequest request,
        ServerCallContext context
    ) => Task.FromResult(new GetUpcomingLessonsResponse());

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
}
