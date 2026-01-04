using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Person.Domain.AggregatesModel.ContactAggregate.Specifications;

public sealed class ContactSpecification : AttributeSpecification<Contact>
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
