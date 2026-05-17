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

    /// <summary>
    /// Возвращает полные детали расписания группы или <c>null</c>, если расписание не создано.
    /// </summary>
    Task<ScheduleDetailDto?> GetScheduleByGroupIdAsync(
        Guid groupId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Возвращает ближайшие <paramref name="count"/> занятий группы начиная с сегодняшней даты.
    /// </summary>
    Task<IReadOnlyList<UpcomingLessonDto>> GetUpcomingLessonsAsync(
        Guid groupId,
        int count = 5,
        CancellationToken cancellationToken = default
    );
}
