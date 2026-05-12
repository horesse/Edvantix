namespace Edvantix.Schedule.Features.GroupSchedules;

/// <summary>Описание временного слота: день недели и время начала в минутах от полуночи.</summary>
public sealed record SlotRequest(int Weekday, int StartMinutes);
