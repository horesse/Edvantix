namespace Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;

/// <summary>Текущий статус учебной группы.</summary>
public enum GroupStatus
{
    Active = 0,
    Recruiting = 1,
    Paused = 2,
    Finished = 3,
    Archived = 4,
}
