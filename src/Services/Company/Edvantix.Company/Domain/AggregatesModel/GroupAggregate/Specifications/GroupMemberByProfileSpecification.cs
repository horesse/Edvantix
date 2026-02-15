using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Company.Domain.AggregatesModel.GroupAggregate.Specifications;

/// <summary>
/// Спецификация для поиска участника группы по идентификатору профиля (опционально в конкретной группе).
/// </summary>
public sealed class GroupMemberByProfileSpecification : CommonSpecification<GroupMember>
{
    public GroupMemberByProfileSpecification(long profileId, long? groupId = null)
    {
        Query.Where(x => x.ProfileId == profileId);

        if (groupId.HasValue)
            Query.Where(x => x.GroupId == groupId.Value);
    }
}
