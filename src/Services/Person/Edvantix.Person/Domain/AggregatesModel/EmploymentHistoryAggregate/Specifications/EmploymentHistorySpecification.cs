using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Builders;

namespace Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate.Specifications;

public sealed class EmploymentHistorySpecification : Specification<EmploymentHistory>
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
            Query.Where(c => c.PersonInfoId == _personId);
    }
}
