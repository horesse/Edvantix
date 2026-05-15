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
}
