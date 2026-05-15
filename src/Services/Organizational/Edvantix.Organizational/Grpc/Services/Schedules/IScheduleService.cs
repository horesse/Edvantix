namespace Edvantix.Organizational.Grpc.Services.Schedules;

public interface IScheduleService
{
    /// <summary>
    /// Возвращает сводки расписаний для нескольких групп одним gRPC-запросом.
    /// Группы без расписания возвращаются с пустым <see cref="ScheduleSummaryDto.SummaryText"/>.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, ScheduleSummaryDto>> GetScheduleSummariesAsync(
        IEnumerable<Guid> groupIds,
        CancellationToken cancellationToken = default
    );
}
