namespace Edvantix.Organizational.Grpc.Services.Schedules;

/// <summary>
/// Детали расписания группы — заполняется при маппинге в Task 8 (Schedule gRPC).
/// Пока возвращается <c>null</c> в <see cref="Features.Groups.GroupDetailDto"/>.
/// </summary>
public sealed record ScheduleDetailDto;
