namespace Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate.Specifications;

/// <summary>Спецификация для загрузки расписания по идентификатору группы (с includes).</summary>
internal sealed class GroupScheduleByGroupIdSpec : Specification<GroupSchedule>
{
    public GroupScheduleByGroupIdSpec(Guid groupId)
    {
        Query.Where(s => s.GroupId == groupId).Include(s => s.Slots).Include(s => s.Exceptions);
    }
}
