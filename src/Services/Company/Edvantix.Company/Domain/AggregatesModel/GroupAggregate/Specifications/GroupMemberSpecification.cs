using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Company.Domain.AggregatesModel.GroupAggregate.Specifications;

/// <summary>
/// Спецификация для поиска участников группы по идентификатору группы.
/// </summary>
public sealed class GroupMemberSpecification : CommonSpecification<GroupMember>
{
    private readonly long? _groupId;

    public long? GroupId
    {
        get => _groupId;
        init
        {
            _groupId = value;
            ApplyWhereExpressions();
        }
    }

    private void ApplyWhereExpressions()
    {
        if (_groupId.HasValue)
            Query.Where(x => x.GroupId == _groupId.Value);
    }
}
