namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Спецификация для поиска участников группы по идентификатору группы.
/// </summary>
public sealed class GroupMemberSpecification : Specification<GroupMember>
{
    public GroupMemberSpecification(Guid? groupId = null, Guid? profileId = null)
    {
        if (groupId.HasValue)
            Query.Where(x => x.GroupId == groupId);

        if (profileId.HasValue)
            Query.Where(x => x.ProfileId == profileId);
    }

    public GroupMemberSpecification() { }

    public GroupMemberSpecification(Guid profileId, Guid? groupId = null)
    {
        Query.Where(x => x.ProfileId == profileId);

        if (groupId.HasValue)
            Query.Where(x => x.GroupId == groupId.Value);
    }
}
