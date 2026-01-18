using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.ProfileService.Domain.AggregatesModel.FullNameAggregate.Specifications;

public sealed class FullNameSpecification : AttributeSpecification<FullName>
{
    private readonly long? _personId;

    public long? PersonId
    {
        get => _personId;
        init
        {
            _personId = value;
            ApplyWhereExpressions();
        }
    }

    private void ApplyWhereExpressions()
    {
        if (_personId.HasValue)
            Query.Where(c => c.ProfileId == _personId);
    }
}
