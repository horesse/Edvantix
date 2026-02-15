using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Company.Domain.AggregatesModel.GroupAggregate.Specifications;

/// <summary>
/// Спецификация для поиска групп, в которых состоит пользователь.
/// </summary>
public sealed class GroupsByProfileSpecification : CommonSpecification<GroupMember>
{
    public GroupsByProfileSpecification(long profileId)
    {
        Query.Where(x => x.ProfileId == profileId);
    }
}
